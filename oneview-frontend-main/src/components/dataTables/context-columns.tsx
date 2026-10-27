import type { ColumnDef } from '@tanstack/react-table';
import styled from 'styled-components';
import { DataTableColumnHeader } from './data-table-column-header';
import { ContextDisplay } from '../ContextDisplay';
import type { AnyContext } from '@/models/context';
import { getContextSearchableValue } from '@/utils/contextHelpers';
import { LabelDisplay } from '../LabelDisplay';
import { useState } from 'react';
import { toast } from 'sonner';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '../ui/dropdown-menu';
import { Button } from '../ui/button';
import { MoreHorizontal } from 'lucide-react';
import { openLabelsDialog } from '../context-labels/useLabelsDialog';

const CellDiv = styled.div`
  text-align: left;
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

export const columns: ColumnDef<AnyContext>[] = [
  // Context name column
  {
    accessorKey: 'name',
    header: ({ column }) => (
      <div className="">
        <DataTableColumnHeader column={column} title="Name" />
      </div>
    ),
    cell: ({ row }) => {
      const type = row.getValue('name') as string | null;
      return (
        <CellDiv>
          {type ? type.charAt(0).toUpperCase() + type.slice(1) : ''}
        </CellDiv>
      );
    },
  },

  // Type column
  {
    accessorKey: 'type',
    header: ({ column }) => (
      <div className="">
        <DataTableColumnHeader column={column} title="Type" />
      </div>
    ),
    cell: ({ row }) => {
      const type = row.getValue('type') as string | null;
      return (
        <CellDiv>
          {type ? type.charAt(0).toUpperCase() + type.slice(1) : ''}
        </CellDiv>
      );
    },
  },

  // Type description column
  {
    accessorKey: 'typeDescription',
    header: ({ column }) => (
      <div className="">
        <DataTableColumnHeader column={column} title="Type Description" />
      </div>
    ),
    cell: ({ row }) => {
      const type = row.getValue('typeDescription') as string | null;
      return (
        <CellDiv>
          {type ? type.charAt(0).toUpperCase() + type.slice(1) : ''}
        </CellDiv>
      );
    },
  },

  // Labels column
  {
    accessorKey: 'labels',
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Labels" />
    ),
    cell: ({ row }) => {
      const ctx = row.original;

      const [labels, setLabels] = useState(ctx.labels ?? []);

      return (
        <LabelDisplay
          labels={labels}
          maxWidth={250}
          onUpdateLabel={async (updatedLabel) => {
            const updated = labels.map((l) =>
              l.customerId + l.name ===
              updatedLabel.customerId + updatedLabel.name
                ? updatedLabel
                : l
            );
            setLabels(updated);
            toast.message('Successfully updated label value');
          }}
        />
      );
    },
  },

  // Context display column, searchable/sortable by name/title
  {
    id: 'context',
    accessorFn: (row) => getContextSearchableValue(row),
    header: ({ column }) => (
      <div className="">
        <DataTableColumnHeader column={column} title="Context" />
      </div>
    ),
    cell: ({ row }) => <ContextDisplay context={row.original} />,
  },

  // CreatedAt column
  {
    accessorKey: 'createdAt',
    header: ({ column }) => (
      <div className="">
        <DataTableColumnHeader column={column} title="Created At" />
      </div>
    ),
    cell: ({ row }) => {
      const iso = row.original.createdAt;
      const date = new Date(iso);
      const createdAt = date.toLocaleDateString();
      return <div className="truncate">{createdAt}</div>;
    },
  },

  // Last Modified column
  {
    accessorKey: 'lastModified',
    header: ({ column }) => (
      <div className="">
        <DataTableColumnHeader column={column} title="Last Modified" />
      </div>
    ),
    cell: ({ row }) => {
      const iso = row.original.lastModified;
      const date = new Date(iso);
      const lastModified = date.toLocaleDateString();
      return <div className="text-right truncate">{lastModified}</div>;
    },
  },
  {
    id: 'actions',
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Actions" className="" />
    ),
    cell: ({ row, table }) => {
      const context = row.original;
      const api = table.options.meta?.api;

      const handleClick = (e: React.MouseEvent) => {
        e.stopPropagation(); // Prevent the row's onClick from firing
      };

      return (
        <div>
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                variant="outline"
                className="h-8 w-8 p-0 focus-border:transparent"
                onClick={handleClick}
                onMouseDown={handleClick}
              >
                <MoreHorizontal className="h-4 w-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" onClick={handleClick}>
              {
                <DropdownMenuItem
                  disabled={!context.referenceLink}
                  onClick={async (e) => {
                    e.stopPropagation();
                    try {
                      const response = await api.post(
                        `/fileserver/files/presigned-url?fileKey=${context.referenceLink}`
                      );
                      const url = response.data;
                      window.location.href = url;
                    } catch (error) {
                      toast.error('Failed to download file. Please try again.');
                    }
                  }}
                >
                  Download file
                </DropdownMenuItem>
              }
              <DropdownMenuSeparator />
              <DropdownMenuItem
                onClick={(e) => {
                  openLabelsDialog(context);
                  e.stopPropagation();
                }}
              >
                Edit labels
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      );
    },
  },
];
