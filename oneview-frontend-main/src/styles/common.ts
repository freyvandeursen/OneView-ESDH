import styled from 'styled-components';

export const CenteredContainer = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  text-align: center;
  height: 90vh;
  gap: 1rem;
`;

// just a simple wrapper to snap ModeToggle to top right corner
// this might be integrated into a navbar or something cool later on
export const ModeToggleWrapper = styled.div`
  position: fixed;
  right: 1rem;
  top: 1rem;
`;
