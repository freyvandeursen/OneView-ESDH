import { ModeToggle } from '@/components/ui/mode-toggle';
import { CenteredContainer, ModeToggleWrapper } from '@/styles/common';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Alert, AlertTitle } from '@/components/ui/alert';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AlertCircleIcon, Loader2 } from 'lucide-react';
import { Logo } from '@/components/Logo';
import api from '@/api/api';
import { AxiosError } from 'axios';

export default function Login() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleLogin = async () => {
    setError('');

    if (!username || !password) {
      setError('Enter username and password.');
      return;
    }

    setLoading(true);

    setTimeout(async () => {
      try {
        const response = await api.post('/Login', {
          username,
          password,
        });

        sessionStorage.setItem('token', response.data.token);
        navigate('/');
      } catch (error) {
        const err = error as AxiosError;

        if (err.response?.status === 400) {
          setError('Invalid username or password.');
          setLoading(false);
        } else {
          setError('An unexpected error occurred.');
          setLoading(false);
        }
      }
    }, 1000);
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      handleLogin();
    }
  };

  return (
    <>
      <ModeToggleWrapper>
        <ModeToggle />
      </ModeToggleWrapper>
      <CenteredContainer>
        <Logo size={300} />
        <Input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          className="w-64"
          onKeyDown={handleKeyDown}
        />
        <Input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="w-64"
          onKeyDown={handleKeyDown}
        />
        <Button variant="outline" onClick={handleLogin} disabled={loading}>
          {loading ? (
            <>
              Logging in...
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
            </>
          ) : (
            'Log in'
          )}
        </Button>
        <div className="h-16 mb-3 flex justify-center items-center relative">
          {error && (
            <Alert variant="destructive">
              <AlertCircleIcon className="h-4 w-4" />
              <AlertTitle>{error}</AlertTitle>
            </Alert>
          )}
        </div>
      </CenteredContainer>
    </>
  );
}
