import { CenteredContainer, ModeToggleWrapper } from '@/styles/common';
import { ModeToggle } from '@/components/ui/mode-toggle';
import { DataTable } from '@/components/dataTables/data-table';
import { useEffect, useState } from 'react';
import type { Customer } from '@/models/customer';
import { useApi } from '@/context/ApiContext';
import { columns } from '@/components/dataTables/all-customers-columns';
import { Button } from '@/components/ui/button';
import { useNavigate } from 'react-router-dom';
import { Plus } from 'lucide-react';

function Customers() {
  const api = useApi();
  const navigate = useNavigate();
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  function handleDeleteCustomer(customerId: string) {
    console.log(`Deleteing ${customerId}`);

    async function deleteCustomer(customerId: string) {
      try {
        const date = new Date().toISOString().split('T')[0];
        const res = await api.delete(`customer/${customerId}/${date}`);

        console.log(
          `Deleted customer ${customerId} at ${date} with status ${res.status}`
        );

        setCustomers((prev) => prev.filter((c) => c.customerId !== customerId));
      } catch (err) {
        console.log(`Failed to delete customer. ${err}`);
        alert(`Failed to delete customer with customer id: ${customerId}`);
      }
    }

    deleteCustomer(customerId);
  }

  useEffect(() => {
    setLoading(true);
    setError('');

    const fetchCustomersWithRetry = async () => {
      for (let i = 0; i < 3; i++) {
        try {
          const res = await api.get('/customer');
          setCustomers(res.data);
          return;
        } catch (err) {
          if (i === 2) {
            setError(
              err instanceof Error ? err.message : 'Failed to load customers'
            );
          } else {
            await new Promise((r) => setTimeout(r, 1000 * 2 ** i));
          }
        }
      }
    };

    fetchCustomersWithRetry().finally(() => {
      setLoading(false); // ← Always runs
    });
  }, [api]);

  return (
    <>
      <ModeToggleWrapper>
        <ModeToggle />
      </ModeToggleWrapper>
      <CenteredContainer>
        <h1>All Customers</h1>
        <DataTable
          data={customers}
          columns={columns}
          height={532}
          loading={loading}
          error={error}
          onDeleteCustomer={handleDeleteCustomer}
          api={api}
          // Create New Customer - Could be extracted to separate component, but KISS for now!
          extraButton1={
            <Button
              variant="outline"
              size="sm"
              className="ml-auto hidden h-8 lg:flex"
              onClick={() => navigate('/newcustomer')}
            >
              <Plus />
              Create New Customer
            </Button>
          }
        />
      </CenteredContainer>
    </>
  );
}

export default Customers;
