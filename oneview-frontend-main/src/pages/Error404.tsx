import { useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/button';
import { CenteredContainer, ModeToggleWrapper } from '@/styles/common';
import { ModeToggle } from '@/components/ui/mode-toggle';

export default function Error404() {
  const navigate = useNavigate();
  return (
    <>
      <ModeToggleWrapper>
        <ModeToggle />
      </ModeToggleWrapper>
      <CenteredContainer>
        <h1>Oops, something went wrong!</h1>
        <Button onClick={() => navigate('/')}>Go Back</Button>
      </CenteredContainer>
    </>
  );
}
