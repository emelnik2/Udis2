
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/28/2022 21:21:07
-- Generated from EDMX file: C:\Users\EduardoM\source\repos\Udis\TenantMNG\Models\TenantModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Udis];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_tbl_invoice_details_tbl_invoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tbl_invoice_details] DROP CONSTRAINT [FK_tbl_invoice_details_tbl_invoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tbl_invoice_tbl_user_master]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tbl_invoice] DROP CONSTRAINT [FK_tbl_invoice_tbl_user_master];
GO
IF OBJECT_ID(N'[dbo].[FK_tbl_pm_billing_hours_tbl_user_master]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tbl_pm_billing_hours] DROP CONSTRAINT [FK_tbl_pm_billing_hours_tbl_user_master];
GO
IF OBJECT_ID(N'[dbo].[FK_tbl_tenant_billing_info_tbl_template]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tbl_tenant_billing_info] DROP CONSTRAINT [FK_tbl_tenant_billing_info_tbl_template];
GO
IF OBJECT_ID(N'[dbo].[FK_tbl_tenant_billing_info_tbl_user_master]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tbl_tenant_billing_info] DROP CONSTRAINT [FK_tbl_tenant_billing_info_tbl_user_master];
GO
IF OBJECT_ID(N'[dbo].[FK_tbl_tenant_contract_tbl_user_master]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tbl_tenant_contract] DROP CONSTRAINT [FK_tbl_tenant_contract_tbl_user_master];
GO
IF OBJECT_ID(N'[dbo].[FK_tbl_tenant_email_setup_tbl_user_master]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tbl_tenant_email_setup] DROP CONSTRAINT [FK_tbl_tenant_email_setup_tbl_user_master];
GO
IF OBJECT_ID(N'[dbo].[FK_tbl_tenant_meter_tbl_user_master]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tbl_tenant_meter] DROP CONSTRAINT [FK_tbl_tenant_meter_tbl_user_master];
GO
IF OBJECT_ID(N'[dbo].[FK_tbl_tenant_settings_tbl_user_master]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tbl_tenant_settings] DROP CONSTRAINT [FK_tbl_tenant_settings_tbl_user_master];
GO
IF OBJECT_ID(N'[dbo].[FK_tbl_user_master_tbl_user_type]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tbl_user_master] DROP CONSTRAINT [FK_tbl_user_master_tbl_user_type];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[tbl_invoice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_invoice];
GO
IF OBJECT_ID(N'[dbo].[tbl_invoice_details]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_invoice_details];
GO
IF OBJECT_ID(N'[dbo].[tbl_pm_billing_hours]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_pm_billing_hours];
GO
IF OBJECT_ID(N'[dbo].[tbl_template]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_template];
GO
IF OBJECT_ID(N'[dbo].[tbl_tenant_billing_info]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_tenant_billing_info];
GO
IF OBJECT_ID(N'[dbo].[tbl_tenant_contract]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_tenant_contract];
GO
IF OBJECT_ID(N'[dbo].[tbl_tenant_email_setup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_tenant_email_setup];
GO
IF OBJECT_ID(N'[dbo].[tbl_tenant_meter]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_tenant_meter];
GO
IF OBJECT_ID(N'[dbo].[tbl_tenant_settings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_tenant_settings];
GO
IF OBJECT_ID(N'[dbo].[tbl_user_master]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_user_master];
GO
IF OBJECT_ID(N'[dbo].[tbl_user_type]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_user_type];
GO
IF OBJECT_ID(N'[dbo].[tbl_zone_master]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_zone_master];
GO
IF OBJECT_ID(N'[DB_TenantMNGModelStoreContainer].[tblMVCCharts]', 'U') IS NOT NULL
    DROP TABLE [DB_TenantMNGModelStoreContainer].[tblMVCCharts];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'tbl_invoice'
CREATE TABLE [dbo].[tbl_invoice] (
    [int_invoice_id] int IDENTITY(1,1) NOT NULL,
    [date_invoice_date] datetime  NULL,
    [int_tenant_id] int  NULL,
    [bit_tenant_active] bit  NULL,
    [date_s_bill_date] datetime  NULL,
    [date_e_bill_date] datetime  NULL,
    [dec_total] decimal(15,5)  NULL,
    [dec_tax_amt] decimal(15,5)  NULL,
    [bit_is_editable] bit  NULL,
    [date_pay_date] datetime  NULL,
    [suministro] decimal(15,5)  NULL,
    [distribucion] decimal(15,5)  NULL,
    [tarifa_transmision] decimal(15,5)  NULL,
    [operacion_cenace] decimal(15,5)  NULL,
    [capacidad] decimal(15,5)  NULL,
    [cre_servicios_conexos] decimal(15,5)  NULL,
    [precio_suministro] decimal(15,5)  NULL,
    [precio_distribucion] decimal(15,5)  NULL,
    [precio_transmision] decimal(15,5)  NULL,
    [precio_cenace] decimal(15,5)  NULL,
    [precio_energia] decimal(15,5)  NULL,
    [precio_capacidad] decimal(15,5)  NULL,
    [precio_cre_servicios_conexos] decimal(15,5)  NULL,
    [precio_dos_porciento_baja_tension] decimal(15,5)  NULL,
    [precio_decuento_bonificacion] decimal(15,5)  NULL
);
GO

-- Creating table 'tbl_invoice_details'
CREATE TABLE [dbo].[tbl_invoice_details] (
    [int_id] int IDENTITY(1,1) NOT NULL,
    [str_meter_id] nvarchar(50)  NULL,
    [int_invoice_id] int  NULL,
    [dec_peak_energy] decimal(15,5)  NULL,
    [dec_peak_energy_rate] decimal(15,5)  NULL,
    [dec_peak_energy_amt] decimal(15,5)  NULL,
    [dec_inter_energy] decimal(15,5)  NULL,
    [dec_inter_energy_rate] decimal(15,5)  NULL,
    [dec_inter_energy_amt] decimal(15,5)  NULL,
    [dec_base_energy] decimal(15,5)  NULL,
    [dec_base_rate] decimal(15,5)  NULL,
    [dec_base_amt] decimal(15,5)  NULL,
    [demanda_base] decimal(15,5)  NULL,
    [demanda_intermedia] decimal(15,5)  NULL,
    [demanda_punta] decimal(15,5)  NULL,
    [energia_activa] decimal(15,5)  NULL,
    [energia_reactiva] decimal(15,5)  NULL
);
GO

-- Creating table 'tbl_pm_billing_hours'
CREATE TABLE [dbo].[tbl_pm_billing_hours] (
    [int_rate_id] int IDENTITY(1,1) NOT NULL,
    [int_pm_id] int  NULL,
    [str_peak_s_time_m] nvarchar(20)  NULL,
    [str_peak_e_time_m] nvarchar(20)  NULL,
    [str_inter_s_time_1_m] nvarchar(20)  NULL,
    [str_inter_e_time_1_m] nvarchar(20)  NULL,
    [str_inter_s_time_2_m] nvarchar(20)  NULL,
    [str_inter_e_time_2_m] nvarchar(20)  NULL,
    [str_base_s_time_m] nvarchar(20)  NULL,
    [str_base_e_time_m] nvarchar(20)  NULL,
    [str_base_s_time_sat] nvarchar(20)  NULL,
    [str_base_e_time_sat] nvarchar(20)  NULL,
    [str_inter_s_time_sat] nvarchar(20)  NULL,
    [str_inter_e_time_sat] nvarchar(20)  NULL,
    [str_base_s_time_sun] nvarchar(20)  NULL,
    [str_base_e_time_sun] nvarchar(20)  NULL,
    [str_inter_s_time_sun] nvarchar(20)  NULL,
    [str_inter_e_time_sun] nvarchar(20)  NULL,
    [str_inter_s_time_2_sat] nvarchar(20)  NULL,
    [str_inter_e_time_2_sat] nvarchar(20)  NULL,
    [str_peak_s_time_sat] nvarchar(20)  NULL,
    [str_peak_e_time_sat] nvarchar(20)  NULL
);
GO

-- Creating table 'tbl_template'
CREATE TABLE [dbo].[tbl_template] (
    [int_temp_id] int IDENTITY(1,1) NOT NULL,
    [str_temp_name] nvarchar(150)  NULL,
    [str_body] nvarchar(max)  NULL
);
GO

-- Creating table 'tbl_tenant_billing_info'
CREATE TABLE [dbo].[tbl_tenant_billing_info] (
    [int_id] int IDENTITY(1,1) NOT NULL,
    [int_tenant_id] int  NULL,
    [int_template_id] int  NULL,
    [dec_rate] decimal(12,5)  NULL,
    [bit_is_seasonal_rate] bit  NULL,
    [dec_seasonal_multi_rate] decimal(12,5)  NOT NULL,
    [bit_is_surchare] bit  NULL,
    [dec_surcharge_amt] decimal(12,5)  NULL,
    [str_min_billable_over] nvarchar(10)  NULL,
    [str_charge_tenant_min] nvarchar(10)  NULL,
    [str_charge_tenant_max] nvarchar(10)  NULL,
    [bit_is_consolidate_zone] bit  NOT NULL,
    [bit_is_print] bit  NOT NULL,
    [bit_is_file] bit  NOT NULL,
    [int_type] int  NOT NULL,
    [str_email] nvarchar(150)  NULL
);
GO

-- Creating table 'tbl_tenant_contract'
CREATE TABLE [dbo].[tbl_tenant_contract] (
    [int_contract_id] int IDENTITY(1,1) NOT NULL,
    [int_tenant_id] int  NULL,
    [s_date] datetime  NULL,
    [e_date] datetime  NULL
);
GO

-- Creating table 'tbl_tenant_email_setup'
CREATE TABLE [dbo].[tbl_tenant_email_setup] (
    [int_email_id] int IDENTITY(1,1) NOT NULL,
    [int_tenant_id] int  NULL,
    [str_from_email] nvarchar(150)  NULL,
    [str_cc_email] nvarchar(150)  NULL,
    [str_bcc_email] nvarchar(150)  NULL,
    [str_subject] nvarchar(150)  NULL,
    [str_body] nvarchar(max)  NULL
);
GO

-- Creating table 'tbl_tenant_meter'
CREATE TABLE [dbo].[tbl_tenant_meter] (
    [int_id] int IDENTITY(1,1) NOT NULL,
    [int_tenant_id] int  NULL,
    [str_meter_id] nvarchar(50)  NULL,
    [date_assign_date] datetime  NULL,
    [date_detach_date] datetime  NULL,
    [bit_is_assign] bit  NULL
);
GO

-- Creating table 'tbl_tenant_settings'
CREATE TABLE [dbo].[tbl_tenant_settings] (
    [int_id] int IDENTITY(1,1) NOT NULL,
    [int_tenant_id] int  NULL,
    [dec_demanda_facturable] decimal(12,5)  NULL,
    [dec_total_ene] decimal(12,5)  NULL
);
GO

-- Creating table 'tbl_user_master'
CREATE TABLE [dbo].[tbl_user_master] (
    [int_id] int IDENTITY(1,1) NOT NULL,
    [str_comp_name] nvarchar(150)  NULL,
    [str_contact_name] nvarchar(150)  NULL,
    [str_add_1] nvarchar(150)  NULL,
    [str_add_2] nvarchar(150)  NULL,
    [str_city] nvarchar(100)  NULL,
    [str_state] nvarchar(50)  NULL,
    [int_pin_code] int  NULL,
    [str_country] nvarchar(50)  NULL,
    [str_email] nvarchar(150)  NULL,
    [int_user_type_id] int  NULL,
    [str_user_name] nvarchar(150)  NULL,
    [str_password] nvarchar(20)  NULL,
    [int_pm_id] int  NULL,
    [date_created] datetime  NULL,
    [dec_last_peak_energy] decimal(10,1)  NULL,
    [dec_last_inter_energy] decimal(10,1)  NULL,
    [int_invoice_period] int  NULL
);
GO

-- Creating table 'tbl_user_type'
CREATE TABLE [dbo].[tbl_user_type] (
    [int_type_id] int IDENTITY(1,1) NOT NULL,
    [str_type] nvarchar(50)  NULL,
    [bit_active] bit  NULL
);
GO

-- Creating table 'tbl_zone_master'
CREATE TABLE [dbo].[tbl_zone_master] (
    [int_zone_id] int IDENTITY(1,1) NOT NULL,
    [str_description] nvarchar(150)  NULL,
    [str_override] nvarchar(100)  NULL,
    [bit_status] bit  NULL
);
GO

-- Creating table 'tblMVCCharts'
CREATE TABLE [dbo].[tblMVCCharts] (
    [ChartID] int IDENTITY(1,1) NOT NULL,
    [Growth_Year] int  NULL,
    [Growth_Value] float  NULL
);
GO

-- Creating table 'UDIS'
CREATE TABLE [dbo].[UDIS] (
    [id_medidor] nvarchar(50)  NOT NULL,
    [CFE_MeterID] nvarchar(50)  NOT NULL,
    [tipo_lectura] nvarchar(50)  NOT NULL,
    [valor] nvarchar(50)  NOT NULL,
    [fecha_ocurrencia] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [int_invoice_id] in table 'tbl_invoice'
ALTER TABLE [dbo].[tbl_invoice]
ADD CONSTRAINT [PK_tbl_invoice]
    PRIMARY KEY CLUSTERED ([int_invoice_id] ASC);
GO

-- Creating primary key on [int_id] in table 'tbl_invoice_details'
ALTER TABLE [dbo].[tbl_invoice_details]
ADD CONSTRAINT [PK_tbl_invoice_details]
    PRIMARY KEY CLUSTERED ([int_id] ASC);
GO

-- Creating primary key on [int_rate_id] in table 'tbl_pm_billing_hours'
ALTER TABLE [dbo].[tbl_pm_billing_hours]
ADD CONSTRAINT [PK_tbl_pm_billing_hours]
    PRIMARY KEY CLUSTERED ([int_rate_id] ASC);
GO

-- Creating primary key on [int_temp_id] in table 'tbl_template'
ALTER TABLE [dbo].[tbl_template]
ADD CONSTRAINT [PK_tbl_template]
    PRIMARY KEY CLUSTERED ([int_temp_id] ASC);
GO

-- Creating primary key on [int_id] in table 'tbl_tenant_billing_info'
ALTER TABLE [dbo].[tbl_tenant_billing_info]
ADD CONSTRAINT [PK_tbl_tenant_billing_info]
    PRIMARY KEY CLUSTERED ([int_id] ASC);
GO

-- Creating primary key on [int_contract_id] in table 'tbl_tenant_contract'
ALTER TABLE [dbo].[tbl_tenant_contract]
ADD CONSTRAINT [PK_tbl_tenant_contract]
    PRIMARY KEY CLUSTERED ([int_contract_id] ASC);
GO

-- Creating primary key on [int_email_id] in table 'tbl_tenant_email_setup'
ALTER TABLE [dbo].[tbl_tenant_email_setup]
ADD CONSTRAINT [PK_tbl_tenant_email_setup]
    PRIMARY KEY CLUSTERED ([int_email_id] ASC);
GO

-- Creating primary key on [int_id] in table 'tbl_tenant_meter'
ALTER TABLE [dbo].[tbl_tenant_meter]
ADD CONSTRAINT [PK_tbl_tenant_meter]
    PRIMARY KEY CLUSTERED ([int_id] ASC);
GO

-- Creating primary key on [int_id] in table 'tbl_tenant_settings'
ALTER TABLE [dbo].[tbl_tenant_settings]
ADD CONSTRAINT [PK_tbl_tenant_settings]
    PRIMARY KEY CLUSTERED ([int_id] ASC);
GO

-- Creating primary key on [int_id] in table 'tbl_user_master'
ALTER TABLE [dbo].[tbl_user_master]
ADD CONSTRAINT [PK_tbl_user_master]
    PRIMARY KEY CLUSTERED ([int_id] ASC);
GO

-- Creating primary key on [int_type_id] in table 'tbl_user_type'
ALTER TABLE [dbo].[tbl_user_type]
ADD CONSTRAINT [PK_tbl_user_type]
    PRIMARY KEY CLUSTERED ([int_type_id] ASC);
GO

-- Creating primary key on [int_zone_id] in table 'tbl_zone_master'
ALTER TABLE [dbo].[tbl_zone_master]
ADD CONSTRAINT [PK_tbl_zone_master]
    PRIMARY KEY CLUSTERED ([int_zone_id] ASC);
GO

-- Creating primary key on [ChartID] in table 'tblMVCCharts'
ALTER TABLE [dbo].[tblMVCCharts]
ADD CONSTRAINT [PK_tblMVCCharts]
    PRIMARY KEY CLUSTERED ([ChartID] ASC);
GO

-- Creating primary key on [id_medidor], [CFE_MeterID], [tipo_lectura], [valor], [fecha_ocurrencia] in table 'UDIS'
ALTER TABLE [dbo].[UDIS]
ADD CONSTRAINT [PK_UDIS]
    PRIMARY KEY CLUSTERED ([id_medidor], [CFE_MeterID], [tipo_lectura], [valor], [fecha_ocurrencia] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [int_invoice_id] in table 'tbl_invoice_details'
ALTER TABLE [dbo].[tbl_invoice_details]
ADD CONSTRAINT [FK_tbl_invoice_details_tbl_invoice]
    FOREIGN KEY ([int_invoice_id])
    REFERENCES [dbo].[tbl_invoice]
        ([int_invoice_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tbl_invoice_details_tbl_invoice'
CREATE INDEX [IX_FK_tbl_invoice_details_tbl_invoice]
ON [dbo].[tbl_invoice_details]
    ([int_invoice_id]);
GO

-- Creating foreign key on [int_tenant_id] in table 'tbl_invoice'
ALTER TABLE [dbo].[tbl_invoice]
ADD CONSTRAINT [FK_tbl_invoice_tbl_user_master]
    FOREIGN KEY ([int_tenant_id])
    REFERENCES [dbo].[tbl_user_master]
        ([int_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tbl_invoice_tbl_user_master'
CREATE INDEX [IX_FK_tbl_invoice_tbl_user_master]
ON [dbo].[tbl_invoice]
    ([int_tenant_id]);
GO

-- Creating foreign key on [int_pm_id] in table 'tbl_pm_billing_hours'
ALTER TABLE [dbo].[tbl_pm_billing_hours]
ADD CONSTRAINT [FK_tbl_pm_billing_hours_tbl_user_master]
    FOREIGN KEY ([int_pm_id])
    REFERENCES [dbo].[tbl_user_master]
        ([int_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tbl_pm_billing_hours_tbl_user_master'
CREATE INDEX [IX_FK_tbl_pm_billing_hours_tbl_user_master]
ON [dbo].[tbl_pm_billing_hours]
    ([int_pm_id]);
GO

-- Creating foreign key on [int_template_id] in table 'tbl_tenant_billing_info'
ALTER TABLE [dbo].[tbl_tenant_billing_info]
ADD CONSTRAINT [FK_tbl_tenant_billing_info_tbl_template]
    FOREIGN KEY ([int_template_id])
    REFERENCES [dbo].[tbl_template]
        ([int_temp_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tbl_tenant_billing_info_tbl_template'
CREATE INDEX [IX_FK_tbl_tenant_billing_info_tbl_template]
ON [dbo].[tbl_tenant_billing_info]
    ([int_template_id]);
GO

-- Creating foreign key on [int_tenant_id] in table 'tbl_tenant_billing_info'
ALTER TABLE [dbo].[tbl_tenant_billing_info]
ADD CONSTRAINT [FK_tbl_tenant_billing_info_tbl_user_master]
    FOREIGN KEY ([int_tenant_id])
    REFERENCES [dbo].[tbl_user_master]
        ([int_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tbl_tenant_billing_info_tbl_user_master'
CREATE INDEX [IX_FK_tbl_tenant_billing_info_tbl_user_master]
ON [dbo].[tbl_tenant_billing_info]
    ([int_tenant_id]);
GO

-- Creating foreign key on [int_tenant_id] in table 'tbl_tenant_contract'
ALTER TABLE [dbo].[tbl_tenant_contract]
ADD CONSTRAINT [FK_tbl_tenant_contract_tbl_user_master]
    FOREIGN KEY ([int_tenant_id])
    REFERENCES [dbo].[tbl_user_master]
        ([int_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tbl_tenant_contract_tbl_user_master'
CREATE INDEX [IX_FK_tbl_tenant_contract_tbl_user_master]
ON [dbo].[tbl_tenant_contract]
    ([int_tenant_id]);
GO

-- Creating foreign key on [int_tenant_id] in table 'tbl_tenant_email_setup'
ALTER TABLE [dbo].[tbl_tenant_email_setup]
ADD CONSTRAINT [FK_tbl_tenant_email_setup_tbl_user_master]
    FOREIGN KEY ([int_tenant_id])
    REFERENCES [dbo].[tbl_user_master]
        ([int_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tbl_tenant_email_setup_tbl_user_master'
CREATE INDEX [IX_FK_tbl_tenant_email_setup_tbl_user_master]
ON [dbo].[tbl_tenant_email_setup]
    ([int_tenant_id]);
GO

-- Creating foreign key on [int_tenant_id] in table 'tbl_tenant_meter'
ALTER TABLE [dbo].[tbl_tenant_meter]
ADD CONSTRAINT [FK_tbl_tenant_meter_tbl_user_master]
    FOREIGN KEY ([int_tenant_id])
    REFERENCES [dbo].[tbl_user_master]
        ([int_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tbl_tenant_meter_tbl_user_master'
CREATE INDEX [IX_FK_tbl_tenant_meter_tbl_user_master]
ON [dbo].[tbl_tenant_meter]
    ([int_tenant_id]);
GO

-- Creating foreign key on [int_tenant_id] in table 'tbl_tenant_settings'
ALTER TABLE [dbo].[tbl_tenant_settings]
ADD CONSTRAINT [FK_tbl_tenant_settings_tbl_user_master]
    FOREIGN KEY ([int_tenant_id])
    REFERENCES [dbo].[tbl_user_master]
        ([int_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tbl_tenant_settings_tbl_user_master'
CREATE INDEX [IX_FK_tbl_tenant_settings_tbl_user_master]
ON [dbo].[tbl_tenant_settings]
    ([int_tenant_id]);
GO

-- Creating foreign key on [int_user_type_id] in table 'tbl_user_master'
ALTER TABLE [dbo].[tbl_user_master]
ADD CONSTRAINT [FK_tbl_user_master_tbl_user_type]
    FOREIGN KEY ([int_user_type_id])
    REFERENCES [dbo].[tbl_user_type]
        ([int_type_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tbl_user_master_tbl_user_type'
CREATE INDEX [IX_FK_tbl_user_master_tbl_user_type]
ON [dbo].[tbl_user_master]
    ([int_user_type_id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------