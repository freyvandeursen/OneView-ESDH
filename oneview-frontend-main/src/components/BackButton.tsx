import { Button } from '@/components/ui/button';
import { ArrowLeft } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import styled from 'styled-components';

export function BackButton() {
  const navigate = useNavigate();

  return (
    <BackButtonWrapper>
      <Button
        variant="outline"
        className="flex items-center gap-2"
        onClick={() => navigate(-1)} // go back one page
      >
        <ArrowLeft className="w-4 h-4" />
        Back
      </Button>
    </BackButtonWrapper>
  );
}

export const BackButtonWrapper = styled.div`
  position: fixed;
  left: 1rem;
  top: 1rem;
`;
