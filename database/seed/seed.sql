INSERT INTO machines (id, name, line_id, station_type) VALUES
('SMT-01', 'SMT Placement Machine 01', 'LINE-A', 'SMT'),
('AOI-01', 'Automated Optical Inspection 01', 'LINE-A', 'AOI'),
('ASSEMBLY-01', 'Assembly Station 01', 'LINE-A', 'ASSEMBLY'),
('TEST-01', 'Electrical Test Station 01', 'LINE-A', 'TEST'),
('PACKING-01', 'Packing Station 01', 'LINE-A', 'PACKING')
ON CONFLICT (id) DO NOTHING;

INSERT INTO work_orders (id, product_code, target_quantity, status, started_at) VALUES
('WO-2026-001', 'PCB-MODULE-A', 1000, 'RUNNING', now())
ON CONFLICT (id) DO NOTHING;
