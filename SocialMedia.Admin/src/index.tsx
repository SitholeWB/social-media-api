
import * as React from 'react';
import * as ReactDOM from 'react-dom/client';
import { StyledEngineProvider } from '@mui/material/styles';
import { GoogleOAuthProvider } from '@react-oauth/google';
import App from './App';

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <GoogleOAuthProvider clientId="YOUR_GOOGLE_CLIENT_ID">
      <StyledEngineProvider injectFirst>
        <App />
      </StyledEngineProvider>
    </GoogleOAuthProvider>
  </React.StrictMode>
);