import { useEffect, useRef, useState } from 'react';
import styled from 'styled-components';
import type { Label } from '@/models/label';
import { SingleLabel } from './SingleLabel';
import { Badge } from './ui/badge';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from './ui/dropdown-menu';

interface LabelDisplayProps {
  labels: Label[];
  maxWidth?: number; // e.g. 250, 300, 400
  onUpdateLabel?: (updated: Label) => void;
}

export function LabelDisplay({
  labels,
  maxWidth = 300,
  onUpdateLabel,
}: LabelDisplayProps) {
  const [visibleCount, setVisibleCount] = useState(labels?.length ?? 0);

  const containerRef = useRef<HTMLDivElement>(null);
  const measureRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!labels || labels.length === 0) return;

    const measureEl = measureRef.current;
    const containerEl = containerRef.current;
    if (!measureEl || !containerEl) return;

    const children = Array.from(measureEl.children);
    let totalWidth = 0;
    let fitCount = 0;

    const containerWidth = containerEl.clientWidth;

    for (let i = 0; i < children.length; i++) {
      const itemWidth = (children[i] as HTMLElement).offsetWidth;

      // Check if adding this tag exceeds the container width (reserve 60px for dropdown)
      if (totalWidth + itemWidth > containerWidth - 60) break;

      totalWidth += itemWidth;
      fitCount++;
    }

    setVisibleCount(fitCount);
  }, [labels, maxWidth]);

  if (!labels || labels.length === 0) {
    return <EmptyLabel></EmptyLabel>;
  }

  const visible = labels.slice(0, visibleCount);
  const overflow = labels.slice(visibleCount);

  return (
    <Wrapper style={{ maxWidth }} ref={containerRef}>
      {/* Visible labels */}
      <LabelRow>
        {visible.map((label, i) => (
          <SingleLabel key={i} label={label} onLabelChange={onUpdateLabel} />
        ))}

        {/* Overflow dropdown */}
        {overflow.length > 0 && (
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <MoreBadge variant="outline">+{overflow.length} more</MoreBadge>
            </DropdownMenuTrigger>
            <DropdownMenuContent>
              {overflow.map((label, i) => (
                <DropdownMenuItem key={i} asChild>
                  <SingleLabel label={label} onLabelChange={onUpdateLabel} />
                </DropdownMenuItem>
              ))}
            </DropdownMenuContent>
          </DropdownMenu>
        )}
      </LabelRow>

      {/* Hidden measurement layer */}
      <MeasureRow ref={measureRef}>
        {labels.map((label, i) => (
          <SingleLabel key={i} label={label} />
        ))}
      </MeasureRow>
    </Wrapper>
  );
}

const Wrapper = styled.div`
  position: relative;
  overflow: hidden;
`;

const LabelRow = styled.div`
  display: flex;
  gap: 0.25rem;
  flex-wrap: nowrap;
  overflow: hidden;
`;

const MoreBadge = styled(Badge)`
  font-size: 0.65rem;
  padding: 0.2rem 0.4rem;
  cursor: pointer;
`;

const MeasureRow = styled.div`
  visibility: hidden;
  position: absolute;
  top: 0;
  left: 0;
  white-space: nowrap;
  pointer-events: none;
`;

const EmptyLabel = styled.span`
  opacity: 0.5;
  font-size: 0.75rem;
`;
