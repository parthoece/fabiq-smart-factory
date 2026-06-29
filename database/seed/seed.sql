INSERT INTO machines
("MachineId", "LineId", "Status", "CurrentWorkOrderId", "GoodCount", "ScrapCount", "LastUpdatedAt")
VALUES
('SMT-01', 'LINE-A', 'RUNNING', 'WO-2026-0001', 0, 0, now()),
('AOI-01', 'LINE-A', 'RUNNING', 'WO-2026-0001', 0, 0, now()),
('ASSEMBLY-01', 'LINE-A', 'RUNNING', 'WO-2026-0001', 0, 0, now()),
('TEST-01', 'LINE-A', 'RUNNING', 'WO-2026-0001', 0, 0, now()),
('PACKING-01', 'LINE-A', 'RUNNING', 'WO-2026-0001', 0, 0, now())
ON CONFLICT ("MachineId") DO NOTHING;

INSERT INTO work_orders
("WorkOrderId", "ProductCode", "LineId", "PlannedQuantity", "GoodCount", "ScrapCount", "Status", "CreatedAt", "StartedAt", "CompletedAt")
VALUES
('WO-2026-0001', 'PCB-MODULE-A', 'LINE-A', 1000, 0, 0, 'RUNNING', now(), now(), NULL)
ON CONFLICT ("WorkOrderId") DO NOTHING;