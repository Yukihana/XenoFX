# XenoFX

A streaming platform with support for storage repository change detection, database API, and media analysis.

- Built on top of ASP.NET Core's robust systems based on TaggableMediaExplorer.
- Uses FFMpeg codec library to ensure high end video quality without compromising on network cost.
- User interface being upgraded to React to ensure fast, modular upgrades.
- Local IP-Pin authentication middleware to control unauthorized access.
- API exposure to allow local analysis applications to benefit from the database.
- Multi-threaded background media analysis and encoding, to ensure content is always ready for the user.

Prototype web interface:

<img width="1872" height="1043" alt="Screenshot 2026-03-03 141718" src="https://github.com/user-attachments/assets/9bfea792-d4b0-4340-a24c-f895a5649865" />

Until larger implementation is possible, Local Ip-Pin ensures controlled access:

<img width="905" height="1045" alt="Screenshot 2026-03-03 141011" src="https://github.com/user-attachments/assets/8dfac134-d3c0-49bd-b6ee-d46349061e14" />

Backend is Serilog enriched:

<img width="605" height="246" alt="Screenshot 2026-03-03 141047" src="https://github.com/user-attachments/assets/a659e23a-d83b-4d02-b430-67f26f6641e7" />

Current Roadmap:

- Redesigning the file indexing core to reflect the functionality of TME, and match the scale of use.
- Standardize supporting services in the backend.
- Add media ratings and metadata databases.
- Rework the user experience.
