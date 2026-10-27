export type Label = {
  id: number;
  customerId: string;
  contextId: number | null;
  name: string;
  description: string | null;
  value: string | null;
  _isOptimistic?: boolean;
};

export const dummyLabels: Label[] = [
  {
    id: 1,
    customerId: 'cust_001',
    contextId: 101,
    name: 'VIP',
    description: 'High-value customer with premium support',
    value: null,
  },
  {
    id: 2,
    customerId: 'cust_001',
    contextId: 102,
    name: 'Tier',
    description: 'Current subscription plan',
    value: 'enterprise',
  },
  {
    id: 3,
    customerId: 'cust_002',
    contextId: null,
    name: 'Churn',
    description: 'At risk of churning',
    value: 'high',
  },
  {
    id: 4,
    customerId: 'cust_003',
    contextId: 103,
    name: 'Industry',
    description: 'Customer industry sector',
    value: 'fintech',
  },
  {
    id: 5,
    customerId: 'cust_004',
    contextId: 104,
    name: 'Onboard',
    description: 'Onboarding completed',
    value: 'done',
  },
  {
    id: 6,
    customerId: 'cust_005',
    contextId: null,
    name: 'Beta',
    description: 'Beta program participant',
    value: 'true',
  },
  {
    id: 7,
    customerId: 'cust_006',
    contextId: 105,
    name: 'Priority',
    description: 'Support ticket priority',
    value: 'P1',
  },
  {
    id: 8,
    customerId: 'cust_007',
    contextId: null,
    name: 'Source',
    description: 'Acquisition channel',
    value: 'partner',
  },
];
