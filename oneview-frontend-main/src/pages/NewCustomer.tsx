import { CenteredContainer, ModeToggleWrapper } from '@/styles/common';
import { ModeToggle } from '@/components/ui/mode-toggle';
import NewCustomerForm from '@/components/newCustomerForm';
import { BackButton } from '@/components/BackButton';

function NewCustomer() {
  return (
    <>
      <ModeToggleWrapper>
        <ModeToggle />
      </ModeToggleWrapper>
      <BackButton />
      <CenteredContainer>
        <NewCustomerForm />
      </CenteredContainer>
    </>
  );
}

export default NewCustomer;
