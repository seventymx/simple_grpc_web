## Simpe gRPC-Web

### Environment Setup

#### 1. **Install Nix (Single-User Mode)**

```pwsh
sh <(curl -L https://nixos.org/nix/install) --no-daemon
```

#### 2. **Enable Nix Flakes (Experimental Feature)**

```pwsh
mkdir -p ~/.config/nix
touch ~/.config/nix/nix.conf
echo "experimental-features = nix-command flakes" >> ~/.config/nix/nix.conf
```

---

### JetBrains Rider Setup (Optional)

If you installed Rider manually (e.g., from a tarball or archive):

-   Extract the downloaded archive to a directory (e.g., `/opt/Rider`).
-   Create symbolic links to make it accessible in the system PATH for terminal usage.

```pwsh
sudo ln -s /opt/Rider/bin/rider.sh /usr/local/bin/rider
```

---

### Nix Flake Development Environment

To enter the Nix flake development environment, run:

```pwsh
nix develop
```

---

### Opening the Project in JetBrains Rider (Non-Blocking Mode)

Open the project solution:

```pwsh
rider . > /dev/null 2>&1 &
```

**Note:** I sometimes need to invalidate the caches and restart Rider to get the project to load correctly. (File > Invalidate Caches...)

---

### Running the Backend

To run the backend service in development mode:

```pwsh
dotnet run
```

**Note:** Navigate to the service directory you want to run (e.g., `cd backend/SPTrialWebServiceCS/`) to run the service.

---

### Frontend Setup

**Note:** The package.json and other frontend configuration files are located in the `root` directory, to make sure eslint and prettier work correctly when the entire project is opened in VS Code. You therefore need to run the following commands from the `root` directory.

#### 1. **Install Frontend Dependencies**

```pwsh
yarn install
```

#### 2. **Generate gRPC-Web Service Stubs**

```pwsh
yarn generate
```

#### 3. **Start Frontend Development Server**

```pwsh
yarn start
```

#### 4. **Build Frontend for Production**

Build artifacts will be stored in the `./frontend/dist` directory:

```pwsh
yarn build
```

---

### gRPC-Web Service (HTTPS Access)

**Note:** Many browsers manage their own certificate stores, so you may need to install the Root CA certificate in your browser to trust the development certificates.

#### Install the Root CA Certificate (Ubuntu)

**1. Copy Certificate to Trusted Store Location**

```pwsh
sudo cp rootCA.pem /usr/local/share/ca-certificates/rootCA.crt
```

**2. Update Trusted Certificates**

```pwsh
sudo update-ca-certificates
```

#### Chromium (or Chrome):

1.  Open Chromium and go to chrome://settings/security.
2.  Scroll down and click Manage certificates.
3.  Go to the Authorities tab.
4.  Click Import and select your rootCA.pem file.
5.  Ensure that the options to Trust this certificate for identifying websites are checked.

#### LibreWolf (or Firefox):

1.  Open LibreWolf and go to about:preferences#privacy.
2.  Scroll down to the Certificates section and click View Certificates.
3.  Go to the Authorities tab and click Import.
4.  Select your rootCA.pem and choose to Trust this CA to identify websites.

### Steps to create a self-signed certificate for localhost

#### 1. **Create a Root CA**

The Root CA will be trusted by your system and browser, and it will sign the development certificates (like `localhost.crt`).

**a. Generate a private key for the Root CA:**

```pwsh
openssl genrsa -out rootCA.key 2048
```

**b. Create a Root CA certificate:**

```pwsh
openssl req -x509 -new -nodes -key rootCA.key -sha256 -days 1024 -out rootCA.pem
```

**Note:** During this process, you’ll be asked to provide information. Make sure to set a **Common Name (CN)** that reflects the purpose of the certificate, such as "Local Dev Root CA".

#### 2. **Create a Certificate for Localhost**

Now, you'll create a certificate for `localhost` signed by the Root CA.

**a. Generate a private key for `localhost`:**

```pwsh
openssl genrsa -out localhost.key 2048
```

**b. Create a certificate signing request (CSR) for `localhost`:**

```pwsh
openssl req -new -key localhost.key -out localhost.csr -config localhost.conf
```

**c. Sign the `localhost` certificate with the Root CA:**

```pwsh
openssl x509 -req -in localhost.csr -CA rootCA.pem -CAkey rootCA.key -CAcreateserial -out localhost.crt -days 500 -sha256 -extfile localhost.conf -extensions req_ext
```

This signs the certificate with the Root CA, which allows it to be trusted as long as the Root CA is trusted.

**d. Create a `.pfx` file for use in development:**

```pwsh
openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt -certfile rootCA.pem
```

Now you can install the Root CA certificate (`rootCA.pem`) as a trusted certificate authority on your system and browsers.
