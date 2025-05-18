USE MaintenanceSystemDB; -- Certifique-se de usar o seu banco de dados correto

-- ? Garantir que o usuário administrador existe
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@empresa.com')
BEGIN
    INSERT INTO Users (Name, Email, PasswordHash, Role)
    VALUES ('Administrador', 'admin@empresa.com', '123456', 0); -- 0 = Admin
    PRINT 'Usuário administrador criado com sucesso.';
END
ELSE
    PRINT 'Usuário administrador já existe. Nenhuma ação foi realizada.';

-- ? Verificar o ID do usuário administrador
DECLARE @AdminUserId INT;
SELECT @AdminUserId = Id FROM Users WHERE Email = 'admin@empresa.com';

-- ? Limpando os dados antigos (APENAS PARA TESTES)
DELETE FROM PlanAssignments;
DELETE FROM Maintenances;
DELETE FROM MaintenancePlans;
DELETE FROM Machines;

-- ? Reiniciando os IDs para começar do 1
DBCC CHECKIDENT ('Machines', RESEED, 0);
DBCC CHECKIDENT ('MaintenancePlans', RESEED, 0);
DBCC CHECKIDENT ('Maintenances', RESEED, 0);
DBCC CHECKIDENT ('PlanAssignments', RESEED, 0);

-- ? Inserindo Máquinas
INSERT INTO Machines (Code, Name, Description, Location, InstallationDate, Status, CreatedByUserId)
VALUES 
('M-001', 'Máquina de Produção', 'Usada para produção contínua', 'Setor A', GETDATE(), 1, @AdminUserId),
('M-002', 'Máquina de Corte', 'Usada para cortes precisos', 'Setor B', GETDATE(), 1, @AdminUserId),
('M-003', 'Máquina de Solda', 'Usada para soldagem de peças', 'Setor C', GETDATE(), 1, @AdminUserId);

-- ? Inserindo Planos de Manutenção
INSERT INTO MaintenancePlans (Title, Description, FrequencyType, FrequencyValue, IsActive, CreatedByUserId)
VALUES 
('Plano Mensal', 'Manutenção mensal para todas as máquinas', 1, 30, 1, @AdminUserId),
('Plano Semanal', 'Manutenção semanal preventiva', 2, 7, 1, @AdminUserId);

-- ? Inserindo Manutenções
INSERT INTO Maintenances (MachineId, Type, PerformedAt, DurationHours, Description, RootCause, CorrectiveAction, CreatedByUserId)
VALUES 
(1, 1, GETDATE(), 2.0, 'Troca de óleo e lubrificação', 'Desgaste normal', 'Troca de óleo', @AdminUserId),
(2, 1, GETDATE(), 3.0, 'Inspeção elétrica', 'Fios soltos', 'Reposição de cabos', @AdminUserId),
(3, 1, GETDATE(), 1.5, 'Verificação de segurança', 'N/A', 'Ajuste de parafusos', @AdminUserId);

-- ? Inserindo Atribuições de Plano
INSERT INTO PlanAssignments (MachineId, MaintenancePlanId, NextDueDate, CreatedByUserId)
VALUES 
(1, 1, DATEADD(DAY, 30, GETDATE()), @AdminUserId), -- Plano Mensal para Máquina 1
(2, 2, DATEADD(DAY, 7, GETDATE()), @AdminUserId),  -- Plano Semanal para Máquina 2
(3, 1, DATEADD(DAY, 30, GETDATE()), @AdminUserId); -- Plano Mensal para Máquina 3

-- ? Verificar os Dados Inseridos
SELECT * FROM Users WHERE Id = @AdminUserId;
SELECT * FROM Machines;
SELECT * FROM MaintenancePlans;
SELECT * FROM Maintenances;
SELECT * FROM PlanAssignments;
