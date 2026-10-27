import { useLocation, useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import type { Customer } from '@/models/customer';
import { useApi } from '@/context/ApiContext';
import { CenteredContainer, ModeToggleWrapper } from '@/styles/common';
import { ModeToggle } from '@/components/ui/mode-toggle';
import { BackButton } from '@/components/BackButton';
import { CustomerInfo } from '@/components/CustomerInfo';
import { CustomerContext } from '@/components/CustomerContext';
import { ContextLabelsDialog } from '@/components/context-labels/ContextLabelsDialog';

export default function CustomerDetails() {
  const location = useLocation();
  const { customerId } = useParams<{ customerId: string }>();
  const api = useApi();

  const [customer, setCustomer] = useState<Customer | null>(
    location.state?.customer || null
  );

  const [loading, setLoading] = useState(!customer);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    // We only fetch the customer if we did not already receive from customersTable (on refresh or via link)
    if (!customer && customerId) {
      setLoading(true);
      api
        .get<Customer>(`/customer/${customerId}`)
        .then((res) => setCustomer(res.data))
        .catch(() => setError('Failed to fetch customer'))
        .finally(() => setLoading(false));
    }
  }, [customer, customerId, api]);

  // Handling of error and loading states - can and should be expanded!
  if (loading) return <div>Loading customer...</div>;
  if (error) return <div>{error}</div>;
  if (!customer) return <div>No customer found</div>;

  return (
    <>
      <ModeToggleWrapper>
        <ModeToggle />
      </ModeToggleWrapper>
      <BackButton />
      <CenteredContainer>
        <div className="flex w-full gap-4 pt-8 h-full">
          <CustomerInfo customer={customer} />
          <CustomerContext customerId={customerId} />
          <ContextLabelsDialog />
        </div>
      </CenteredContainer>
    </>
  );
}
