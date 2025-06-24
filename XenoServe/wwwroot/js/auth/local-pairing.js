/**
 * @typedef {Object} ClientInfoResponse
 * @property {string} currentIp
 * @property {boolean} isAuthorized
 * @property {string} tokenExpiresAt
 * @property {string} activeIp
 * @property {number} leaseStartTime
 * @property {string} leaseEndTime
*/

export class XsIpPinAuthClient {
    // Login form elements
    #pinField = undefined;
    #systemMessageText = undefined; // all exceptions will be shown here

    // Login/logout elements
    #loginStateText = undefined; // will show 'logged in until {expiry}' or 'logged out'
    #logoutBtn = undefined;

    // Ip
    #ipSection = undefined;
    #currentIpText = undefined;
    #activeIpText = undefined;
    #leaseText = undefined;

    // internal parameters
    #currentMessageId = 0;

    // Properties

    get partialApiPath() {
        return '/api/auth/ip-pin-auth/';
    }

    // System

    showMessage = async (message, type) => {
        this.#systemMessageText.innerHtml = message;
        if (type == 'error') {
            this.#systemMessageText.style.color = '#f99'; // red
        } else if (type == 'success') {
            this.#systemMessageText.style.color = '#0f0'; // green
        } else if (type == 'info') {
            this.#systemMessageText.style.color = '#fff'; // white
        } else if (type == 'warning') {
            this.#systemMessageText.style.color = '#ff0'; // yellow
        } else {
            this.#systemMessageText.style.color = '#aaa'; // gray
        }

        // Show message, allow events, and wait for duration before revert
        const currentMessageId = ++this.#currentMessageId;
        this.#systemMessageText.style.opacity = 1;
        this.#systemMessageText.pointerEvents = 'auto';
        await new Promise(resolve => setTimeout(resolve, duration));

        // Ensure same message id, then hide message
        if (currentMessageId === this.#currentMessageId) {
            this.#systemMessageText.style.opacity = 0;
            this.#systemMessageText.pointerEvents = 'none';
        }
    };

    handle = async (response) => {
        if (response.ok) return;

        let message = `Request failed with status ${response.status}`;
        try {
            const contentType = response.headers.get("Content-Type") || "";
            if (contentType.includes("application/json")) {
                const errorBody = await response.json();
                message = errorBody?.error || JSON.stringify(errorBody);
            } else {
                message = await response.text();
            }
        } catch {
            // fallback message stays as-is
        }

        const error = new Error(message);
        error.status = response.status;
        throw error;
    };

    // Endpoints --------------
    // State

    syncState = async (event) => {
        // request state and apply it
        try {
            const response = await fetch(getPartialApiPath + 'status', {
                method: 'POST',
                credentials: 'include', // attach httponly cookies
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({}) // no body needed
            });

            // Handle errors (throws if !response.ok)
            await this.handle(response);

            // Parse JSON body (assumes valid camelCase payload)
            /** @type {ClientInfoResponse} */
            const payload = await response.json();

            // Apply state
            this.setState(
                payload.currentIp,
                payload.isAuthorized,
                payload.tokenExpiresAt
            );
            this.setIp(
                payload.isAuthorized,
                payload.activeIp,
                payload.leaseStartTime,
                payload.leaseEndTime
            );
        } catch (error) {
            console.error(error.message);
            this.showMessage(error.message, 'error');
        }
    };

    // Session

    generatePin = async (event) => {
        try {
            // request pin
            const response = await fetch(getPartialApiPath + 'generate', {
                method: 'POST',
                credentials: 'include', // attach httponly cookies
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({}) // no body needed
            });

            // Handle response and sync
            await this.handle(response);
            await this.syncState(event);
        } catch (error) {
            console.error(error.message);
            this.showMessage(error.message, 'error');
        }
    };

    verifyPin = async (event) => {
        try {
            // get submitted pin
            const pin = this.#pinField.value;
            if (!pin) {
                this.showMessage('Please enter a PIN.', 'error');
                return;
            }

            // send pin to server
            const response = await fetch(getPartialApiPath + 'authorize', {
                method: 'POST',
                credentials: 'include', // attach httponly cookies
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(pin) // send the pin as secure data
            });

            // Handle response and sync
            await this.handle(response);
            await this.syncState();
        } catch (error) {
            console.error(error.message);
            this.showMessage(error.message, 'error');
        }
    }

    logOut = async (event) => {
        try {
            // send log out request
            const response = await fetch(getPartialApiPath + 'logout', {
                method: 'POST',
                credentials: 'include', // attach httponly cookies
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({}) // no body needed
            });

            // Handle response and sync
            await this.handle(response);
            await this.syncState();
        } catch (error) {
            console.error(error.message);
            this.showMessage(error.message, 'error');
        }
    }

    // Ip

    activateIp = async (event) => {
        try {
            // request to activate a lease for the current ip
            const response = await fetch(getPartialApiPath + 'activate-ip', {
                method: 'POST',
                credentials: 'include', // attach httponly cookies
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({}) // no body needed
            });

            // Handle response and sync
            await this.handle(response);
            await this.syncState();
        } catch (error) {
            console.error(error.message);
            this.showMessage(error.message, 'error');
        }
    };

    deactivateIp = async (event) => {
        try {
            // request to deactivate the lease for the current ip
            const response = await fetch(getPartialApiPath + 'deactivate-ip', {
                method: 'POST',
                credentials: 'include', // attach httponly cookies
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({}) // no body needed
            });

            // Handle response and sync
            await this.handle(response);
            await this.syncState();
        } catch (error) {
            console.error(error.message);
            this.showMessage(error.message, 'error');
        }
    };

    // UI

    setState = (currentIp, authenticated, expiry) => {
        this.#currentIpText.innerHTML = currentIp || 'Error detecting the ip address';

        if (authenticated) {
            this.#logoutBtn.style.display = 'block';
            this.#loginStateText.innerHtml = `Authenticated until ${expiry}`;
            this.#loginStateText.style.color = 'green'; // green
        } else {
            this.#logoutBtn.style.display = 'none';
            this.#loginStateText.innerHtml = 'Not logged in';
            this.#loginStateText.style.color = '#f99'; // red
        }
    }

    setIp = (authenticated, activeIp, startTime, endTime) => {
        this.#ipSection.style.display = authenticated ? 'block' : 'none';

        if (activeIp) {
            this.#activeIpText.innerHTML = activeIp;
            this.#activeIpText.style.color = 'white';
            this.#leaseText.innerHTML = `Lease: ${startTime} - ${endTime}`;
            this.#leaseText.style.display = 'block';
            this.#activateIpText.innerHtml = 'Renew';
        } else {
            this.#activeIpText.innerHTML = 'Not set';
            this.#activeIpText.style.color = '#999';
            this.#leaseText.style.display = 'none';
            this.#activateIpText.innerHtml = 'Activate';
        }
    }

    // Setup

    setupInteractions = () => {
        // Logged status
        this.#logoutBtn.addEventListener('click', event => this.logOut(event));
        // Pin
        document.querySelector('#cpbtn-generate-pin').addEventListener('click', event => this.generatePin(event));
        document.querySelector('#cpbtn-authorize').addEventListener('click', event => this.verifyPin(event));
        // Ip
        document.querySelector('#cpbtn-activate-ip').addEventListener('click', event => this.activateIp(event));
        document.querySelector('#cpbtn-deactivate-ip').addEventListener('click', event => this.deactivateIp(event));
    }
    setupElements = () => {
        // Status
        #currentIpText = document.querySelector('#cp-current-ip-text');
        #loginStateText = document.querySelector('#cp-login-state-text'); // will show 'logged in until {expiry}' or 'logged out'
        #logoutBtn = document.querySelector('#cpbtn-logout');
        // Login
        #pinField = document.querySelector('#cpform-auth-pin-field');
        #systemMessageText = document.querySelector('#cp-message-box-text'); // all exceptions will be shown here
        // Ip
        #ipSection = document.querySelector('#cpsection-active-ip');
        #activeIpText = document.querySelector('#cp-active-ip-text');
        #leaseText = document.querySelector('#cp-active-ip-lease-text');
    }

    // Bootstrap

    initialize = () => {
        this.setupElements();
        this.setupInteractions();
        this.checkStatus();
    }
}