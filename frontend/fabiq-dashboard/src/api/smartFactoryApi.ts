import axios from "axios";

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5078";
  
export const api = axios.create({
  baseURL: API_BASE_URL,
});

export type Machine = {
  id: number;
  machineId: string;
  lineId: string;
  status: string;
  currentWorkOrderId?: string | null;
  goodCount: number;
  scrapCount: number;
  lastUpdatedAt: string;
};

export type OeeResponse = {
  lineId?: string;
  workOrderId?: string;
  availability: number;
  performance: number;
  quality: number;
  oee: number;
  goodCount: number;
  scrapCount: number;
  totalCount: number;
};

export type DowntimeSummary = {
  lineId: string;
  reasonCode: string;
  eventCount: number;
  totalDurationMinutes: number;
  affectedMachineCount: number;
  lastOccurredAt: string;
};

export type ProductionEvent = {
  eventId: string;
  machineId: string;
  workOrderId: string;
  lineId: string;
  eventType: string;
  partId?: string | null;
  quantity: number;
  defectType?: string | null;
  notes?: string | null;
  createdAt: string;
};

export type PartTraceability = {
  partId: string;
  finalStatus: string;
  currentMachineId: string;
  currentLineId: string;
  lastSeenAt: string;
  route: PartTraceabilityStep[];
};

export type PartTraceabilityStep = {
  eventId: string;
  machineId: string;
  lineId: string;
  workOrderId: string;
  productCode?: string | null;
  eventType: string;
  quantity: number;
  defectType?: string | null;
  notes?: string | null;
  createdAt: string;
};

export async function getMachines() {
  const response = await api.get<Machine[]>("/api/machines/status");
  return response.data;
}

export async function getLineOee(lineId: string) {
  const response = await api.get<OeeResponse>(`/api/oee/line/${lineId}`, {
    params: {
      plannedProductionMinutes: 480,
      idealCycleTimeSeconds: 30,
    },
  });

  return response.data;
}

export async function getDowntimeSummary(lineId: string) {
  const response = await api.get<DowntimeSummary[]>("/api/downtime/summary", {
    params: { lineId },
  });

  return response.data;
}

export async function getRecentProductionEvents() {
  const response = await api.get<ProductionEvent[]>("/api/productionevents/recent", {
    params: { limit: 100 },
  });

  return response.data;
}

export async function getPartTraceability(partId: string) {
  const response = await api.get<PartTraceability>(
    `/api/traceability/part/${partId}`
  );

  return response.data;
}