USE MaintenanceSystemDB; -- Certifique-se de usar o seu banco de dados correto

-- ? Garantir que o usu�rio administrador existe
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@empresa.com')
BEGIN
    INSERT INTO Users (Name, Email, PasswordHash, Role)
    VALUES ('Administrador', 'admin@empresa.com', '123456', 0); -- 0 = Admin
    PRINT 'Usu�rio administrador criado com sucesso.';
END
ELSE
    PRINT 'Usu�rio administrador j� existe. Nenhuma a��o foi realizada.';

-- ? Verificar o ID do usu�rio administrador
DECLARE @AdminUserId INT;
SELECT @AdminUserId = Id FROM Users WHERE Email = 'admin@empresa.com';

-- ? Limpando os dados antigos (APENAS PARA TESTES)
DELETE FROM PlanAssignments;
DELETE FROM Maintenances;
DELETE FROM MaintenancePlans;
DELETE FROM Machines;

-- ? Reiniciando os IDs para come�ar do 1
DBCC CHECKIDENT ('Machines', RESEED, 0);
DBCC CHECKIDENT ('MaintenancePlans', RESEED, 0);
DBCC CHECKIDENT ('Maintenances', RESEED, 0);
DBCC CHECKIDENT ('PlanAssignments', RESEED, 0);

-- ? Inserindo M�quinas
INSERT INTO Machines (Code, Name, Description, Location, InstallationDate, Status, CreatedByUserId)
VALUES 
('M-001', 'M�quina de Produ��o', 'Usada para produ��o cont�nua', 'Setor A', GETDATE(), 1, @AdminUserId),
('M-002', 'M�quina de Corte', 'Usada para cortes precisos', 'Setor B', GETDATE(), 1, @AdminUserId),
('M-003', 'M�quina de Solda', 'Usada para soldagem de pe�as', 'Setor C', GETDATE(), 1, @AdminUserId);

-- ? Inserindo Planos de Manuten��o
INSERT INTO MaintenancePlans (Title, Description, FrequencyType, FrequencyValue, IsActive, CreatedByUserId)
VALUES 
('Plano Mensal', 'Manuten��o mensal para todas as m�quinas', 1, 30, 1, @AdminUserId),
('Plano Semanal', 'Manuten��o semanal preventiva', 2, 7, 1, @AdminUserId);

-- ? Inserindo Manuten��es
INSERT INTO Maintenances (MachineId, Type, PerformedAt, DurationHours, Description, RootCause, CorrectiveAction, CreatedByUserId)
VALUES 
(1, 1, GETDATE(), 2.0, 'Troca de �leo e lubrifica��o', 'Desgaste normal', 'Troca de �leo', @AdminUserId),
(2, 1, GETDATE(), 3.0, 'Inspe��o el�trica', 'Fios soltos', 'Reposi��o de cabos', @AdminUserId),
(3, 1, GETDATE(), 1.5, 'Verifica��o de seguran�a', 'N/A', 'Ajuste de parafusos', @AdminUserId);

-- ? Inserindo Atribui��es de Plano
INSERT INTO PlanAssignments (MachineId, MaintenancePlanId, NextDueDate, CreatedByUserId)
VALUES 
(1, 1, DATEADD(DAY, 30, GETDATE()), @AdminUserId), -- Plano Mensal para M�quina 1
(2, 2, DATEADD(DAY, 7, GETDATE()), @AdminUserId),  -- Plano Semanal para M�quina 2
(3, 1, DATEADD(DAY, 30, GETDATE()), @AdminUserId); -- Plano Mensal para M�quina 3

-- ? Verificar os Dados Inseridos
SELECT * FROM Users WHERE Id = @AdminUserId;
SELECT * FROM Machines;
SELECT * FROM MaintenancePlans;
SELECT * FROM Maintenances;
SELECT * FROM PlanAssignments;
