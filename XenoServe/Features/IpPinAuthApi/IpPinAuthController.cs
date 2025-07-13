using CSX.Common.Data.Exceptions;
using CSX.Common.Data.Placeholders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Auth.IpPinAuth;
using XenoFx.Services.Auth.IpPinAuth.Models;
using XenoServe.Features.IpPinAuthApi.Payloads;

namespace XenoServe.Features.IpPinAuthApi;

[Route("auth/ip-pin-auth")]
[ApiController]
public class IpPinAuthController : ControllerBase
{
    // Constants

    public const string DeviceIdHeaderKey = "xenoserve-device-id";
    public const string AuthTokenHeaderKey = "xenoserve-auth-token";
    public const string DeviceStatusHeaderKey = "xenoserve-device-status";
    public const string TokenExpiryHeaderKey = "xenoserve-token-expiry";
    public const string AuthPinHeaderKey = "xenoserve-auth-pin";

    // Infrastructure

    private readonly IIpPinAuthService _ipPinAuth;
    private readonly ILogger<IpPinAuthController> _logger;

    // Lifecycle

    public IpPinAuthController(
        IIpPinAuthService ipPinAuth,
        ILogger<IpPinAuthController> logger)
    {
        _ipPinAuth = ipPinAuth;
        _logger = logger;
    }

    // Shared custom scaffolding

    private async Task<IActionResult> SafeExecuteEndpointAsync<TQuery, TResult>(
        Func<IpPinAuthDto<TQuery, TResult>, CancellationToken, Task<IActionResult>> func,
        CancellationToken ctoken = default)
    {
        IpPinAuthDto<TQuery, TResult>? dto = null;

        try
        {
            ctoken.ThrowIfCancellationRequested();

            // ensure valid remote ip
            IPAddress ip = HttpContext.Connection.RemoteIpAddress
                ?? throw new MalformedIpException("Bad remote ip address.");

            // Build dto
            dto = new IpPinAuthDto<TQuery, TResult>(ip: ip)
            {
                ClientId = Request.Cookies[DeviceIdHeaderKey]?.Trim().ToLowerInvariant(),   // Hexdec, normalized
                AuthToken = Request.Cookies[AuthTokenHeaderKey]?.Trim()                     // Base64url, case sensitive
            };

            // Introduce artificial delay to prevent timed attacks
            Task delay = Task.Delay(500 + Random.Shared.Next(0, 500), ctoken);
            Task<IActionResult> actual = func(dto, ctoken);
            await Task.WhenAll(delay, actual);
            var response = await actual;

            // Post-process : Device Id
            if (dto.NewClientId is string newDeviceId &&
                !string.IsNullOrWhiteSpace(newDeviceId))
            {
                Response.Cookies.Append(DeviceIdHeaderKey, newDeviceId, new CookieOptions
                {
                    HttpOnly = true,
                    //Secure = true,                  // Required for HTTPS
                    SameSite = SameSiteMode.Strict, // or Lax, depending on your needs
                    Expires = DateTimeOffset.UtcNow.AddYears(20)
                });
            }

            // Finally return the response
            return response;
        }
        catch (MalformedIpException ex)
        {
            _logger.LogWarning(ex, "Bad remote address: {ip}. Endpoint: '{path}'",
                HttpContext.Connection.RemoteIpAddress,
                Request.Path);
            return Unauthorized(ex.Message);
        }
        catch (ClientDisabledException ex)
        {
            _logger.LogWarning(ex, "Client with banned IP: {ip}. Endpoint: '{path}'",
                HttpContext.Connection.RemoteIpAddress,
                Request.Path);
            return Unauthorized(ex.Message);
        }
        catch (MissingHttpHeaderException ex)
        {
            _logger.LogWarning(ex, "Missing header in request from ip {ip} at path {path}. Headers: {headers}",
                HttpContext.Connection.RemoteIpAddress,
                Request.Path,
                Request.Headers);
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Unauthorized access attempt at endpoint '{path}' from the ip {ip}.",
                HttpContext.Connection.RemoteIpAddress,
                Request.Path);
            return Unauthorized("Unauthorized access");
        }
        catch (ServiceFailureException ex)
        {
            _logger.LogError(ex, "Service failure occured at endpoint '{path}' for the ip {ip}. Details: @{dto}",
                HttpContext.Connection.RemoteIpAddress,
                Request.Path,
                dto);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Service unavailable");
        }
        catch (Exception ex)
        when (ex is not OperationCanceledException
        and not TaskCanceledException)
        {
            _logger.LogError(ex, "Error executing request from {dto}", dto);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    // Note: cannot use [Authorize] for /auth endpoints.
    // It would prevent users from logging in.

    // Endpoints: with payload in response body

    [HttpPost("status")]
    public async Task<IActionResult> GetStatusAsync(
        CancellationToken ctoken = default)
    {
        return await SafeExecuteEndpointAsync<Unit, IpPinAuthClientInfo>(async (dto, ct) =>
        {
            // Execute endpoint task
            await _ipPinAuth.GetStatusAsync(dto, ct);

            // Ensure the result isn't missing
            if (dto.ResponsePayload is not IpPinAuthClientInfo result)
                throw new ServiceFailureException("Failed to retrieve session info");

            // Map the response and deliver
            ClientInfoResponse payload = dto.ResponsePayload.MapToResponse();
            return Ok(payload);
        }, ctoken);
    }

    // Endpoints: strictly without response body

    [HttpPost("generate")]
    public async Task<IActionResult> GeneratePinAsync(
        CancellationToken ctoken = default)
    {
        return await SafeExecuteEndpointAsync<Unit, int>(async (dto, ct) =>
        {
            // Generate a pin
            await _ipPinAuth.GeneratePinAsync(dto, ct);

            // Output the pin (in this case, log)
            _logger.LogInformation(
                "Pin {pin} generated for {ip}",
                dto.ResponsePayload,
                dto.IPAddress);

            return Ok();
        }, ctoken);
    }

    [HttpPost("authorize")]
    public async Task<IActionResult> SubmitPinAsync(
        [FromBody] int pin,
        CancellationToken ctoken = default)
    {
        return await SafeExecuteEndpointAsync<int?, IpPinAuthentication>(async (dto, ct) =>
        {
            // Attach the pin read from the body
            dto.RequestPayload = pin;

            // Verify the pin sent in the request body
            await _ipPinAuth.LoginAsync(dto, ctoken);

            // Assume success from this point on. Log it.
            _logger.LogInformation("IP {ip} has been authenticated.", dto.IPAddress);

            string authToken = dto.ResponsePayload
                .AuthToken.Trim();
            DateTimeOffset expiry = dto.ResponsePayload
                .Expiry;

            // Write token to cookie (doesn't need to be handled JS side, further post requests will include this)
            Response.Cookies.Append(AuthTokenHeaderKey, authToken, new CookieOptions
            {
                HttpOnly = true,
                // Secure = true, // Https only
                SameSite = SameSiteMode.Strict,
                Expires = expiry,
            });

            // Finish
            return Ok();
        }, ctoken);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(
        CancellationToken ctoken = default)
    {
        return await SafeExecuteEndpointAsync<Unit, Unit>(async (dto, ct) =>
        {
            await _ipPinAuth.LogoutAsync(dto, ctoken);

            return Ok();
        }, ctoken);
    }

    [HttpPost("activate-ip")]
    public async Task<IActionResult> ActivateIpAsync(
        [FromBody] TimeSpan? duration,
        CancellationToken ctoken = default)
    {
        return await SafeExecuteEndpointAsync<TimeSpan?, Unit>(async (dto, ct) =>
        {
            // Expected behavior: noValue = default, 0 = max
            dto.RequestPayload = duration;

            await _ipPinAuth.ActivateIpAsync(dto, ctoken);

            return Ok();
        }, ctoken);
    }

    [HttpPost("deactivate-ip")]
    public async Task<IActionResult> DeactivateIpAsync(
        CancellationToken ctoken = default)
    {
        return await SafeExecuteEndpointAsync<Unit, Unit>(async (dto, ct) =>
        {
            await _ipPinAuth.DeactivateIpAsync(dto, ctoken);

            return Ok();
        }, ctoken);
    }
}