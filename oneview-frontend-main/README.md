# OneView - ESDH: Frontend

The React frontend for **OneView**, an ESDH-inspired prototype for customer and document management. The interface brings customer records, contacts, labels, and related documents into a unified workspace.

OneView is a group project developed by **six people in collaboration with Vitani**. This React frontend is part of the team's shared work.

See the workspace [local setup and run guide](../README.md) for prerequisites, backend/database startup, authentication secrets, demo logins, verification, and backups.

With the API running at `http://localhost:8080`:

```powershell
pnpm install --frozen-lockfile
pnpm dev --host 127.0.0.1 --port 5173 --strictPort
```

Open http://127.0.0.1:5173/login. Run these commands from `oneview-frontend-main`.
