import type { Customer } from '@/models/customer';
import type { ColumnDef } from '@tanstack/react-table';
import { MoreHorizontal } from 'lucide-react';
import { Button } from '@/components/ui/button';
import styled from 'styled-components';
import { useNavigate } from 'react-router-dom';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { DataTableColumnHeader } from './data-table-column-header';
import '@tanstack/react-table';

const CellDiv = styled.div`
  text-align: left;
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

export const columns: ColumnDef<Customer>[] = [
  {
    accessorKey: 'customerId',
    header: ({ column }) => (
      <div className="w-20">
        <DataTableColumnHeader column={column} title="ID" />
      </div>
    ),
    cell: ({ row }) => {
      const customerId = row.getValue('customerId') as string | null;
      return <CellDiv>{customerId ?? ''}</CellDiv>;
    },
  },
  {
    accessorKey: 'name',
    header: ({ column }) => (
      <div className="w-80">
        <DataTableColumnHeader column={column} title="Name" />
      </div>
    ),
    cell: ({ row }) => {
      return (
        <div className="text-left font-medium truncate">
          {row.getValue('name')}
        </div>
      );
    },
  },
  {
    accessorKey: 'address',
    header: ({ column }) => (
      <div className="w-80">
        <DataTableColumnHeader column={column} title="Address" />
      </div>
    ),
    cell: ({ row }) => {
      return (
        <div className="text-left font-medium truncate">
          {row.getValue('address')}
        </div>
      );
    },
  },
  {
    accessorKey: 'phoneNumber',
    header: ({ column }) => (
      <div className="w-40">
        <DataTableColumnHeader column={column} title="Phone" />
      </div>
    ),
    cell: ({ row }) => {
      return (
        <div className="text-left font-medium truncate">
          {row.getValue('phoneNumber')}
        </div>
      );
    },
  },
  {
    accessorKey: 'email',
    header: ({ column }) => (
      <div className="w-40">
        <DataTableColumnHeader column={column} title="Email" />
      </div>
    ),
    cell: ({ row }) => {
      return (
        <div className="text-left font-medium truncate">
          {row.getValue('email')}
        </div>
      );
    },
  },
  {
    accessorKey: 'vatNumber',
    header: ({ column }) => (
      <div className="w-40">
        <DataTableColumnHeader column={column} title="VAT Number" />
      </div>
    ),
    cell: ({ row }) => {
      return (
        <div className="text-left font-medium">{row.getValue('vatNumber')}</div>
      );
    },
  },
  {
    accessorKey: 'deactivationDate',
    enableGlobalFilter: false, // this column will not impact searching
    header: ({ column }) => (
      <div className="w-40">
        <DataTableColumnHeader column={column} title="Deactivation" />
      </div>
    ),
    cell: ({ row }) => {
      return (
        <div className="text-left font-medium">
          {row.getValue('deactivationDate')}
        </div>
      );
    },
  },
  {
    id: 'actions',
    header: ({ column }) => (
      <div className="w-10">
        <DataTableColumnHeader column={column} title="" />
      </div>
    ),
    cell: ({ row, table }) => {
      const navigate = useNavigate();
      const customer = row.original;
      const onDelete = table.options.meta?.onDeleteCustomer;

      const handleClick = (e: React.MouseEvent) => {
        e.stopPropagation(); // Prevent the row's onClick from firing
      };

      return (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button
              variant="outline"
              className="h-8 w-8 p-0 -translate-x-5 focus-border:transparent"
              onClick={handleClick}
              onMouseDown={handleClick}
            >
              <MoreHorizontal className="h-4 w-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" onClick={handleClick}>
            {customer.customerId != null && (
              <DropdownMenuItem
                onClick={(e) => {
                  e.stopPropagation();
                  navigator.clipboard.writeText(customer.customerId!);
                }}
              >
                Copy customer ID
              </DropdownMenuItem>
            )}
            <DropdownMenuSeparator />
            <DropdownMenuItem
              onClick={(e) => {
                e.stopPropagation();
                const customer = row.original as {
                  customerId?: string;
                };
                if (customer.customerId) {
                  navigate(`/customers/${customer.customerId}`, {
                    state: { customer },
                  });
                }
              }}
            >
              View customer
            </DropdownMenuItem>
            <DropdownMenuItem
              onClick={(e) => {
                e.stopPropagation();
              }}
            >
              Edit customer details
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem
              onClick={(e) => {
                e.stopPropagation();
                if (confirm(`Delete ${customer.name}?`)) {
                  onDelete?.(customer.customerId!);
                }
              }}
            >
              Delete customer
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      );
    },
  },
];
