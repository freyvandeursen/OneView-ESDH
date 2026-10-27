import styled from 'styled-components';
import type { Label } from '@/models/label';
import { Badge } from './ui/badge';
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from './ui/tooltip';
import { useState } from 'react';
import { EditLabelDialog } from './edit-label-dialog';

interface SingleLabelProps {
  label: Label;
  onLabelChange?: (updated: Label) => void;
}

export function SingleLabel({ label, onLabelChange }: SingleLabelProps) {
  const [showDialog, setShowDialog] = useState(false);

  return (
    <>
      <TooltipProvider delayDuration={300}>
        <Tooltip>
          <TooltipTrigger asChild>
            <LabelBadge variant="outline" onClick={() => setShowDialog(true)}>
              {label.name}
              {label.value ? `: ${label.value}` : ''}
            </LabelBadge>
          </TooltipTrigger>

          {label.description && (
            <TooltipContent side="top" className="text-sm">
              {label.description}
            </TooltipContent>
          )}
        </Tooltip>
      </TooltipProvider>

      <EditLabelDialog
        open={showDialog}
        onClose={() => setShowDialog(false)}
        initialValue={label.value!}
        onSave={(newValue) => {
          onLabelChange?.({
            ...label,
            value: newValue,
          });
        }}
      />
    </>
  );
}

const LabelBadge = styled(Badge)`
  font-size: 0.65rem;
  margin-right: 0.4rem;
  padding: 0.2rem 0.4rem;
  font-weight: 500;
  white-space: nowrap;
  cursor: pointer;
  transition:
    background-color 0.15s ease,
    border-color 0.15s ease;

  &:hover {
    background-color: rgba(0, 0, 0, 0.08); /* same subtle hover as before */
    border-color: rgba(0, 0, 0, 0.2);
  }
`;
