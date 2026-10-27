import { DataTable } from './dataTables/data-table';
import { columns } from './dataTables/context-columns';
import { Card } from './ui/card';
import { Button } from './ui/button';
import { CustomerLabels } from './customer-labels/CustomerLabels';
import { useEffect, useState } from 'react';
import { useApi } from '@/context/ApiContext';
import { toast } from 'sonner';
import type { Context } from '@/models/context';
import type { S3File } from '@/models/s3file';

export function CustomerContext({
  customerId,
}: {
  customerId: string | undefined;
}) {
  const [loading, _setLoading] = useState(false);
  const [error, _setError] = useState('');

  const [contexts, setContexts] = useState<Context<S3File>[]>([]);
  const api = useApi();

  useEffect(() => {
    if (!customerId) return;
    api
      .get(`/Context/${customerId}`)
      .then((res) => {
        const mapped = res.data.map((ctx: any) => ({
          name: ctx.name,
          type: ctx.contextType?.typeName,
          typeDescription: ctx.contextType?.description,
          labels: ctx.labels ?? [],
          context: ctx.contextData ?? [],
          createdAt: ctx.createdAt,
          lastModified: ctx.lastUpdated,
          referenceLink: ctx.referenceLink,
          customerId: ctx.customerId,
          contextId: ctx.contextId,
          data: {
            name: ctx.name,
            url: ctx.referenceLink,
            type: ctx.contextData?.[0]?.value,
          },
        }));
        setContexts(mapped);
      })
      .catch(() =>
        toast.error(`Failed to fetch contexts for customer ${customerId}`)
      );
  }, [customerId, api]);

  return (
    <div className="flex-3 flex flex-col gap-2">
      <Card className="flex-1 shadow-md flex items-center justify-center">
        <p className="text-sm text-muted-foreground">What goes here?</p>
        <div className="flex gap-4">
          <CustomerLabels customerId={customerId} />
          <Button variant={'outline'}>Edit Customer</Button>
          <Button variant={'destructive'}>Delete Customer</Button>
        </div>
        <p className="text-sm text-muted-foreground">KPI?</p>
        <p className="text-sm text-muted-foreground">
          Number of context items for this customer: {contexts.length}
        </p>
      </Card>
      <div className="mt-2">
        <DataTable
          data={contexts}
          columns={columns}
          height={572}
          loading={loading}
          error={error}
          api={api}
        />
      </div>
    </div>
  );
}
