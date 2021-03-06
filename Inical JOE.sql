USE [master]

go

CREATE DATABASE [DBJOE]
GO
USE [DBJOE]
GO
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Instituicoes] (
    [Id] int NOT NULL IDENTITY,
    [Descricao] nvarchar(max) NULL,
    [IsAtivo] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Instituicoes] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [TiposUser] (
    [Id] int NOT NULL IDENTITY,
    [Descricao] nvarchar(max) NULL,
    [IsAtivo] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_TiposUser] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Cursos] (
    [Id] int NOT NULL IDENTITY,
    [Descricao] nvarchar(max) NULL,
    [InstituicaoId] int NOT NULL,
    [IsAtivo] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Cursos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Cursos_Instituicoes_InstituicaoId] FOREIGN KEY ([InstituicaoId]) REFERENCES [Instituicoes] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Empresas] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NULL,
    [RazaoSocial] nvarchar(max) NULL,
    [CNPJ] nvarchar(max) NULL,
    [Endereco] nvarchar(max) NULL,
    [Telefone] nvarchar(max) NULL,
    [Site] nvarchar(max) NULL,
    [AreaAtuacao] nvarchar(max) NULL,
    [Servico] nvarchar(max) NULL,
    [IsAtivo] bit NOT NULL DEFAULT 1,
    [InstituicaoId] int NULL,
    CONSTRAINT [PK_Empresas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Empresas_Instituicoes_InstituicaoId] FOREIGN KEY ([InstituicaoId]) REFERENCES [Instituicoes] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Usuarios] (
    [Id] int NOT NULL IDENTITY,
    [TipoId] int NOT NULL,
    [InstituicaoId] int NOT NULL,
    [Nome] nvarchar(max) NULL,
    [Matricula] nvarchar(max) NULL,
    [Senha] nvarchar(max) NULL,
    [Cidade] nvarchar(max) NULL,
    [Endereco] nvarchar(max) NULL,
    [Telefone] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [DateAdd] datetime2 NOT NULL DEFAULT ((getdate())),
    [LastAcesso] datetime2 NULL,
    [IsAtivo] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Usuarios_Instituicoes_InstituicaoId] FOREIGN KEY ([InstituicaoId]) REFERENCES [Instituicoes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Usuarios_TiposUser_TipoId] FOREIGN KEY ([TipoId]) REFERENCES [TiposUser] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [OrientadoresEstagiarios] (
    [Id] int NOT NULL IDENTITY,
    [EstagiarioId] int NOT NULL,
    [OrientadorId] int NOT NULL,
    [EmpresaId] int NOT NULL,
    [Supervisor] nvarchar(max) NULL,
    [Previsao] datetime2 NOT NULL,
    [IsAtivo] bit NOT NULL DEFAULT 1,
    [IsFinalizado] bit NOT NULL DEFAULT 0,
    CONSTRAINT [PK_OrientadoresEstagiarios] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrientadoresEstagiarios_Empresas_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresas] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrientadoresEstagiarios_Usuarios_EstagiarioId] FOREIGN KEY ([EstagiarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrientadoresEstagiarios_Usuarios_OrientadorId] FOREIGN KEY ([OrientadorId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [UsuariosCurso] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [CursoId] int NOT NULL,
    CONSTRAINT [PK_UsuariosCurso] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UsuariosCurso_Cursos_CursoId] FOREIGN KEY ([CursoId]) REFERENCES [Cursos] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UsuariosCurso_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Relatorios] (
    [Id] int NOT NULL IDENTITY,
    [EstagioId] int NOT NULL,
    [ResponsavelId] int NOT NULL,
    [Descricao] nvarchar(max) NULL,
    [NameFile] nvarchar(max) NULL,
    [DateAdd] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_Relatorios] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Relatorios_OrientadoresEstagiarios_EstagioId] FOREIGN KEY ([EstagioId]) REFERENCES [OrientadoresEstagiarios] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Relatorios_Usuarios_ResponsavelId] FOREIGN KEY ([ResponsavelId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_Cursos_InstituicaoId] ON [Cursos] ([InstituicaoId]);

GO

CREATE INDEX [IX_Empresas_InstituicaoId] ON [Empresas] ([InstituicaoId]);

GO

CREATE INDEX [IX_OrientadoresEstagiarios_EmpresaId] ON [OrientadoresEstagiarios] ([EmpresaId]);

GO

CREATE INDEX [IX_OrientadoresEstagiarios_EstagiarioId] ON [OrientadoresEstagiarios] ([EstagiarioId]);

GO

CREATE INDEX [IX_OrientadoresEstagiarios_OrientadorId] ON [OrientadoresEstagiarios] ([OrientadorId]);

GO

CREATE INDEX [IX_Relatorios_EstagioId] ON [Relatorios] ([EstagioId]);

GO

CREATE INDEX [IX_Relatorios_ResponsavelId] ON [Relatorios] ([ResponsavelId]);

GO

CREATE INDEX [IX_Usuarios_InstituicaoId] ON [Usuarios] ([InstituicaoId]);

GO

CREATE INDEX [IX_Usuarios_TipoId] ON [Usuarios] ([TipoId]);

GO

CREATE INDEX [IX_UsuariosCurso_CursoId] ON [UsuariosCurso] ([CursoId]);

GO

CREATE INDEX [IX_UsuariosCurso_UsuarioId] ON [UsuariosCurso] ([UsuarioId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20181211140424_Initial', N'2.1.4-rtm-31024');

INSERT INTO Instituicoes(Descricao) VALUES('IFG - URUACU - Thiane'),('IFG - URUACU - Rodrigo'),('IFG - URUACU - Lynwood'),('IFG - URUACU - Alessandro'),('IFG - URUACU - Kevym');INSERT INTO TiposUser(Descricao) VALUES('Admin'),('Professor'),('Aluno')INSERT INTO USUARIOS(TipoId, InstituicaoId,Nome, Matricula,Senha,Cidade,Endereco,Telefone, Email) VALUES(1,1,'Thiane Marques Torquato', '123456','123','Uruacu','Centro','',''),(1,2,'Rodrigo Geppex', '12345','123','Uruacu','Centro','',''),(1,3,'Lynwood Livi de Souza', '1234','123','Uruacu','Centro','',''),(1,4,' Alessandro Siqueira da Silva', '123','123','Uruacu','Centro','',''),(1,5,'Kevym Carlos Bastista Anacleto', '123456789','123','Uruacu','Centro','','');
go
USE [master]
GO
CREATE LOGIN [joeadmin] WITH PASSWORD=N'systemjoe', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [DBJOE]
GO
CREATE USER [joeadmin] FOR LOGIN [joeadmin]
GO
USE [DBJOE]
GO
ALTER ROLE [db_owner] ADD MEMBER [joeadmin]
GO
