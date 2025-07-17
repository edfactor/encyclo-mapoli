export interface Health {
  status: string;
  totalDuration: string;
  entries: Entries;
}

export interface Entries {
  OracleHcm: DemoulasCommonDataContext;
  process_allocated_memory: DemoulasCommonDataContext;
  "masstransit-bus": MasstransitBus;
  ProfitSharingDbContext: DemoulasCommonDataContext;
  ProfitSharingReadOnlyDbContext: DemoulasCommonDataContext;
  DemoulasCommonDataContext: DemoulasCommonDataContext;
  self: DemoulasCommonDataContext;
  Environment: Environment;
}

export interface DemoulasCommonDataContext {
  duration: string;
  status: string;
  tags: string[];
  description?: string;
}

export interface Environment {
  data: EnvironmentData;
  description: string;
  duration: string;
  status: string;
}

export interface EnvironmentData {
  Environment: string;
  ApplicationName: string;
  MachineName: string;
  OSVersion: string;
  Framework: string;
  AppVersion: string;
  CurrentDirectory: string;
  Uptime: string;
  UtcNow: Date;
  OktaEnvironmentName: string;
}

export interface MasstransitBus {
  description: string;
  duration: string;
  status: string;
  tags: string[];
}
