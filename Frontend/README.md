# 🌐 NashTech Rookie Storefront - Frontend Application

Welcome to the **NashTech Rookie Storefront** frontend application. This is a modern, high-performance web application built with **Next.js 16 (App Router)**, **React 19**, **Tailwind CSS 4**, and **Redux Toolkit**.

To ensure secure communications with the backend services, the development environment runs fully on **HTTPS** using locally generated SSL certificates.

---

## 🛠️ Tech Stack & Key Libraries

- **Framework**: [Next.js 16 (App Router)](https://nextjs.org/)
- **Library**: [React 19](https://react.dev/)
- **State Management**: [Redux Toolkit](https://redux-toolkit.js.org/) & [React Redux](https://react-redux.js.org/)
- **Styling & UI**: [Tailwind CSS v4](https://tailwindcss.com/) & [DaisyUI v5](https://daisyui.com/)
- **Animations**: [Motion](https://motion.dev/)
- **Forms & Validation**: [React Hook Form](https://react-hook-form.com/) & [Zod](https://zod.dev/)

---

## 🚀 Getting Started

Follow these steps to set up and run the frontend application on your local machine.

### Prerequisites

Ensure you have the following installed on your machine:

- **Node.js**: `v20.x` or higher (LTS recommended)
- **npm**: `v10.x` or higher

---

### 1. Install Dependencies

Navigate to the `Frontend` directory and install the required npm packages:

```bash
cd Frontend
npm install
```

---

### 2. Environment Variables & Secrets Configuration

The application separates general configuration from sensitive credentials (secrets).

#### A. Local Frontend Environment Configuration (`.env`)

In the `Frontend` folder, you will find an `.env.example` file.

1. Create a copy of `.env.example` and name it `.env.development` (or `.env.local` for local-only overrides):
   ```bash
   cp .env.example .env.development
   ```
2. Open `.env.development` and populate the required variables:

| Variable Name                     | Description                            | Recommended Local Value                                  |
| :-------------------------------- | :------------------------------------- | :------------------------------------------------------- |
| `NEXT_PUBLIC_API_URL`             | Base URL for the Backend API endpoint  | `https://localhost:5001` _(or your custom backend port)_ |
| `NEXT_PUBLIC_SESSION_COOKIE_NAME` | Name of the secure session/auth cookie | `session_token` _(or as specified by backend)_           |

> [!NOTE]
> Variables prefixed with `NEXT_PUBLIC_` are safely exposed to the browser. Do **NOT** store raw private API keys, database passwords, or private certificates in these variables.

#### B. Centralized Secrets Directory (`/secrets`)

For backend services, Docker Compose, or local database credentials, the project contains a centralized `secrets/` folder in the **root of the workspace**:

```
📁 Rookies2026Batch9Net1/
├── 📁 Backend/
├── 📁 Frontend/
└── 📁 secrets/          <-- Centralized secrets folder
    ├── 📄 example.env
    └── 📄 nam-db.env    (sensitive DB credentials)
```

1. **Download Secrets**: Obtain the current `.env` files and certificates from the shared **Google Drive** folder (under the `secrets` directory).
2. **Place in Workspace**: Paste these files directly into the root `secrets/` directory of your local repository.
3. **Database & Infrastructure**: These secrets will be automatically read by docker-compose or local backend configurations during startup.

---

### 3. HTTPS Certificates Setup

To run the frontend securely over HTTPS in development, Next.js utilizes SSL certificates.

The certificates are located in:
`Frontend/certificates/localhost.pem` and `Frontend/certificates/localhost-key.pem`

- **Why is this needed?**: The development script utilizes `--experimental-https` (`next dev --experimental-https`), allowing Next.js to serve the application locally over HTTPS instead of standard HTTP.
- **Security**: These local PEM certificates are git-ignored (`Frontend/.gitignore`) to maintain repository security. Make sure they are placed in `Frontend/certificates/` (obtained from Google Drive or generated via `mkcert`).

---

### 4. Running the Development Server

Start the development server using the following command:

```bash
npm run dev
```

Once started, the CLI will output the active URL. Open your browser and navigate to:
👉 **[https://localhost:3000](https://localhost:3000)**

> [!TIP]
> If your browser warns you about an "untrusted certificate", this is normal for local development certificates. You can safely proceed/bypass the warning, or trust the `localhost.pem` certificate in your system keychain.

---

## 📦 Available Scripts

In the `Frontend` directory, you can run the following scripts:

| Command              | Action                                                                         |
| :------------------- | :----------------------------------------------------------------------------- |
| `npm run dev`        | Starts the Next.js development server on **HTTPS** (`https://localhost:3000`). |
| `npm run build`      | Compiles and builds the production-ready optimized bundle.                     |
| `npm run start`      | Starts the Next.js production server (run after `npm run build`).              |
| `npm run lint`       | Runs ESLint to check for code quality and syntax issues.                       |
| `npm run type-check` | Runs the TypeScript compiler (`tsc`) to verify type safety.                    |
