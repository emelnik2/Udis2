Use [DB_TenantMNG]
/****** Object:  Table [dbo].[tbl_invoice]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_invoice](
	[int_invoice_id] [int] IDENTITY(1,1) NOT NULL,
	[date_invoice_date] [date] NULL,
	[int_meter_id] [int] NULL,
	[int_tenant_id] [int] NULL,
	[bit_tenant_active] [bit] NULL,
	[date_s_bill_date] [date] NULL,
	[date_e_bill_date] [date] NULL,
	[dec_peak_energy] [decimal](10, 2) NULL,
	[dec_peak_energy_rate] [decimal](10, 2) NULL,
	[dec_inter_energy] [decimal](10, 2) NULL,
	[dec_inter_energy_rate] [decimal](10, 2) NULL,
	[dec_custome_charges] [decimal](10, 2) NOT NULL,
	[dec_demad] [decimal](10, 2) NULL,
	[dec_total] [decimal](10, 2) NULL,
	[date_modify] [date] NULL,
	[dec_tax_amt] [decimal](10, 2) NULL,
	[bit_is_editable] [bit] NULL,
	[date_pay_date] [date] NULL,
	[str_custome_charge_desc] [nvarchar](150) NULL,
	[dec_prev_peack_energy] [decimal](10, 1) NULL,
	[dec_prev_inter_energy] [decimal](10, 1) NULL,
	[dec_current_peack_energy] [decimal](10, 1) NULL,
	[dec_current_inter_energy] [decimal](10, 1) NULL,
 CONSTRAINT [PK_tbl_invoice_1] PRIMARY KEY CLUSTERED 
(
	[int_invoice_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_pm_billing_hours]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_pm_billing_hours](
	[int_rate_id] [int] IDENTITY(1,1) NOT NULL,
	[int_pm_id] [int] NULL,
	[str_peak_s_time] [nvarchar](20) NULL,
	[str_peak_e_time] [nvarchar](20) NULL,
	[str_inter_s_time] [nvarchar](20) NULL,
	[str_inter_e_time] [nvarchar](20) NULL,
 CONSTRAINT [PK_tbl_pm_billing_hours] PRIMARY KEY CLUSTERED 
(
	[int_rate_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_template]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_template](
	[int_temp_id] [int] IDENTITY(1,1) NOT NULL,
	[str_temp_name] [nvarchar](150) NULL,
	[str_body] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_template] PRIMARY KEY CLUSTERED 
(
	[int_temp_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_tenant_billing_info]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tenant_billing_info](
	[int_id] [int] IDENTITY(1,1) NOT NULL,
	[int_tenant_id] [int] NULL,
	[int_template_id] [int] NULL,
	[dec_rate] [decimal](7, 2) NULL,
	[bit_is_seasonal_rate] [bit] NULL,
	[dec_seasonal_multi_rate] [decimal](7, 2) NOT NULL,
	[bit_is_surchare] [bit] NULL,
	[dec_surcharge_amt] [decimal](7, 2) NULL,
	[str_min_billable_over] [nvarchar](10) NULL,
	[str_charge_tenant_min] [nvarchar](10) NULL,
	[str_charge_tenant_max] [nvarchar](10) NULL,
	[bit_is_consolidate_zone] [bit] NOT NULL,
	[bit_is_print] [bit] NOT NULL,
	[bit_is_file] [bit] NOT NULL,
	[int_type] [int] NOT NULL,
	[str_email] [nvarchar](150) NULL,
 CONSTRAINT [PK_tbl_tenant_master] PRIMARY KEY CLUSTERED 
(
	[int_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_tenant_contract]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tenant_contract](
	[int_contract_id] [int] IDENTITY(1,1) NOT NULL,
	[int_tenant_id] [int] NULL,
	[s_date] [date] NULL,
	[e_date] [date] NULL,
 CONSTRAINT [PK_tbl_tenant_contract] PRIMARY KEY CLUSTERED 
(
	[int_contract_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_tenant_email_setup]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tenant_email_setup](
	[int_email_id] [int] IDENTITY(1,1) NOT NULL,
	[int_tenant_id] [int] NULL,
	[str_from_email] [nvarchar](150) NULL,
	[str_cc_email] [nvarchar](150) NULL,
	[str_bcc_email] [nvarchar](150) NULL,
	[str_subject] [nvarchar](150) NULL,
	[str_body] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_tenant_email_setup] PRIMARY KEY CLUSTERED 
(
	[int_email_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_tenant_meter]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tenant_meter](
	[int_id] [int] IDENTITY(1,1) NOT NULL,
	[int_tenant_id] [int] NULL,
	[int_meter_id] [int] NULL,
	[date_assign_date] [date] NULL,
	[date_detach_date] [date] NULL,
	[bit_is_assign] [bit] NULL,
 CONSTRAINT [PK_tbl_tenant_meter] PRIMARY KEY CLUSTERED 
(
	[int_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_user_master]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_master](
	[int_id] [int] IDENTITY(1,1) NOT NULL,
	[str_comp_name] [nvarchar](150) NULL,
	[str_contact_name] [nvarchar](150) NULL,
	[str_add_1] [nvarchar](150) NULL,
	[str_add_2] [nvarchar](150) NULL,
	[str_city] [nvarchar](100) NULL,
	[str_state] [nvarchar](50) NULL,
	[int_pin_code] [int] NULL,
	[str_country] [nvarchar](50) NULL,
	[str_email] [nvarchar](150) NULL,
	[int_user_type_id] [int] NULL,
	[str_user_name] [nvarchar](150) NULL,
	[str_password] [nvarchar](20) NULL,
	[int_pm_id] [int] NULL,
	[date_created] [date] NULL,
	[dec_last_peak_energy] [decimal](10, 1) NULL,
	[dec_last_inter_energy] [decimal](10, 1) NULL,
	[int_invoice_period] [int] NULL,
 CONSTRAINT [PK_tbl_property_manager] PRIMARY KEY CLUSTERED 
(
	[int_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_user_type]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_type](
	[int_type_id] [int] IDENTITY(1,1) NOT NULL,
	[str_type] [nvarchar](50) NULL,
	[bit_active] [bit] NULL,
 CONSTRAINT [PK_tbl_admin] PRIMARY KEY CLUSTERED 
(
	[int_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_zone_master]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_zone_master](
	[int_zone_id] [int] IDENTITY(1,1) NOT NULL,
	[str_description] [nvarchar](150) NULL,
	[str_override] [nvarchar](100) NULL,
	[bit_status] [bit] NULL,
 CONSTRAINT [PK_tbl_zone_master] PRIMARY KEY CLUSTERED 
(
	[int_zone_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblMVCCharts]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblMVCCharts](
	[ChartID] [int] IDENTITY(1,1) NOT NULL,
	[Growth_Year] [int] NULL,
	[Growth_Value] [float] NULL
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[tbl_template] ON 

GO
INSERT [dbo].[tbl_template] ([int_temp_id], [str_temp_name], [str_body]) VALUES (1, N'Default Template', N'<h1>Hello</h1>')
GO
SET IDENTITY_INSERT [dbo].[tbl_template] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_user_master] ON 

GO
INSERT [dbo].[tbl_user_master] ([int_id], [str_comp_name], [str_contact_name], [str_add_1], [str_add_2], [str_city], [str_state], [int_pin_code], [str_country], [str_email], [int_user_type_id], [str_user_name], [str_password], [int_pm_id], [date_created], [dec_last_peak_energy], [dec_last_inter_energy], [int_invoice_period]) VALUES (1, N'XcessLogic', N'Admin', N'Delhi', NULL, N'Delhi', N'Delhi', NULL, N'India', NULL, 1, N'admin', N'admin', 0, CAST(N'2017-07-05' AS Date), CAST(0.0 AS Decimal(10, 1)), CAST(0.0 AS Decimal(10, 1)), 3)
GO
SET IDENTITY_INSERT [dbo].[tbl_user_master] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_user_type] ON 

GO
INSERT [dbo].[tbl_user_type] ([int_type_id], [str_type], [bit_active]) VALUES (1, N'Admin', 1)
GO
INSERT [dbo].[tbl_user_type] ([int_type_id], [str_type], [bit_active]) VALUES (2, N'Property Manager', 1)
GO
INSERT [dbo].[tbl_user_type] ([int_type_id], [str_type], [bit_active]) VALUES (3, N'Tenant', 1)
GO
SET IDENTITY_INSERT [dbo].[tbl_user_type] OFF
GO
SET IDENTITY_INSERT [dbo].[tblMVCCharts] ON 

GO
INSERT [dbo].[tblMVCCharts] ([ChartID], [Growth_Year], [Growth_Value]) VALUES (1, 2008, 50)
GO
INSERT [dbo].[tblMVCCharts] ([ChartID], [Growth_Year], [Growth_Value]) VALUES (2, 2009, 70)
GO
INSERT [dbo].[tblMVCCharts] ([ChartID], [Growth_Year], [Growth_Value]) VALUES (3, 2010, 80)
GO
INSERT [dbo].[tblMVCCharts] ([ChartID], [Growth_Year], [Growth_Value]) VALUES (4, 2011, 90)
GO
INSERT [dbo].[tblMVCCharts] ([ChartID], [Growth_Year], [Growth_Value]) VALUES (5, 2012, 120)
GO
INSERT [dbo].[tblMVCCharts] ([ChartID], [Growth_Year], [Growth_Value]) VALUES (6, 2013, 150)
GO
INSERT [dbo].[tblMVCCharts] ([ChartID], [Growth_Year], [Growth_Value]) VALUES (7, 2014, 100)
GO
INSERT [dbo].[tblMVCCharts] ([ChartID], [Growth_Year], [Growth_Value]) VALUES (8, 2015, 300)
GO
SET IDENTITY_INSERT [dbo].[tblMVCCharts] OFF
GO
ALTER TABLE [dbo].[tbl_invoice]  WITH CHECK ADD  CONSTRAINT [FK_tbl_invoice_tbl_user_master] FOREIGN KEY([int_tenant_id])
REFERENCES [dbo].[tbl_user_master] ([int_id])
GO
ALTER TABLE [dbo].[tbl_invoice] CHECK CONSTRAINT [FK_tbl_invoice_tbl_user_master]
GO
ALTER TABLE [dbo].[tbl_pm_billing_hours]  WITH CHECK ADD  CONSTRAINT [FK_tbl_pm_billing_hours_tbl_user_master] FOREIGN KEY([int_pm_id])
REFERENCES [dbo].[tbl_user_master] ([int_id])
GO
ALTER TABLE [dbo].[tbl_pm_billing_hours] CHECK CONSTRAINT [FK_tbl_pm_billing_hours_tbl_user_master]
GO
ALTER TABLE [dbo].[tbl_tenant_billing_info]  WITH CHECK ADD  CONSTRAINT [FK_tbl_tenant_billing_info_tbl_template] FOREIGN KEY([int_template_id])
REFERENCES [dbo].[tbl_template] ([int_temp_id])
GO
ALTER TABLE [dbo].[tbl_tenant_billing_info] CHECK CONSTRAINT [FK_tbl_tenant_billing_info_tbl_template]
GO
ALTER TABLE [dbo].[tbl_tenant_billing_info]  WITH CHECK ADD  CONSTRAINT [FK_tbl_tenant_billing_info_tbl_user_master] FOREIGN KEY([int_tenant_id])
REFERENCES [dbo].[tbl_user_master] ([int_id])
GO
ALTER TABLE [dbo].[tbl_tenant_billing_info] CHECK CONSTRAINT [FK_tbl_tenant_billing_info_tbl_user_master]
GO
ALTER TABLE [dbo].[tbl_tenant_contract]  WITH CHECK ADD  CONSTRAINT [FK_tbl_tenant_contract_tbl_user_master] FOREIGN KEY([int_tenant_id])
REFERENCES [dbo].[tbl_user_master] ([int_id])
GO
ALTER TABLE [dbo].[tbl_tenant_contract] CHECK CONSTRAINT [FK_tbl_tenant_contract_tbl_user_master]
GO
ALTER TABLE [dbo].[tbl_tenant_email_setup]  WITH CHECK ADD  CONSTRAINT [FK_tbl_tenant_email_setup_tbl_user_master] FOREIGN KEY([int_tenant_id])
REFERENCES [dbo].[tbl_user_master] ([int_id])
GO
ALTER TABLE [dbo].[tbl_tenant_email_setup] CHECK CONSTRAINT [FK_tbl_tenant_email_setup_tbl_user_master]
GO
ALTER TABLE [dbo].[tbl_tenant_meter]  WITH CHECK ADD  CONSTRAINT [FK_tbl_tenant_meter_tbl_user_master] FOREIGN KEY([int_tenant_id])
REFERENCES [dbo].[tbl_user_master] ([int_id])
GO
ALTER TABLE [dbo].[tbl_tenant_meter] CHECK CONSTRAINT [FK_tbl_tenant_meter_tbl_user_master]
GO
ALTER TABLE [dbo].[tbl_user_master]  WITH CHECK ADD  CONSTRAINT [FK_tbl_user_master_tbl_user_type] FOREIGN KEY([int_user_type_id])
REFERENCES [dbo].[tbl_user_type] ([int_type_id])
GO
ALTER TABLE [dbo].[tbl_user_master] CHECK CONSTRAINT [FK_tbl_user_master_tbl_user_type]
GO
/****** Object:  StoredProcedure [dbo].[tbl_insert_zone]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[tbl_insert_zone] 
(
@str_decription nvarchar(150),
@str_override nvarchar(100),
@bit_status bit
)	
AS
BEGIN
	
	insert into tbl_zone_master values(@str_decription,@str_override,@bit_status)
	select @@IDENTITY


END

GO
/****** Object:  StoredProcedure [dbo].[usp_change_password]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_change_password]
(
@int_id int,
@str_new_password nvarchar(150)
)
AS
BEGIN
update tbl_user_master set 
str_password=@str_new_password
where int_id=@int_id

select 1
END

GO
/****** Object:  StoredProcedure [dbo].[usp_change_profile]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_change_profile] 
(
@int_id int,
@str_comp_name nvarchar(150),
@str_contact_name nvarchar(150),
@str_add_1 nvarchar(150),
@str_add_2 nvarchar(150),
@str_city nvarchar(150),
@str_state nvarchar(150),
@int_pin_code int,
@str_country nvarchar(150),
@str_email nvarchar(150)
)	
AS
BEGIN
	update tbl_user_master set 
	str_comp_name=@str_comp_name,
	str_contact_name=@str_contact_name,
	str_add_1=@str_add_1,
	str_add_2=@str_add_2,
	str_city=@str_city,
	str_state=@str_state,
	str_email=@str_email,
	str_country=@str_country,
	int_pin_code=@int_pin_code
	where int_id=@int_id


END

GO
/****** Object:  StoredProcedure [dbo].[usp_delete_user]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_user] 
(
@int_id int
)
AS
BEGIN
	delete from [dbo].[tbl_user_master] where int_id=@int_id
END

GO
/****** Object:  StoredProcedure [dbo].[usp_detach_meter]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_detach_meter]
(
@int_id int
)
AS
BEGIN
	update tbl_tenant_meter set 
	date_detach_date=GETDATE(),bit_is_assign='False'
	where int_tenant_id=@int_id and bit_is_assign='True'

	select 1
END

GO
/****** Object:  StoredProcedure [dbo].[usp_insert_invoice]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_invoice]
(
@int_id int OUT,
@int_meter_id int,
@int_tenant_id int,
@bit_tenant_active bit,
@date_s_bill_date date,
@date_e_bill_date date,
@dec_peak_energy decimal(10,2),
@dec_peak_energy_rate decimal(10,2),
@dec_inter_energy decimal(10,2),
@dec_inter_energy_rate decimal(10,2),
@dec_custome_charges decimal(10,2),
@dec_demad decimal(10,2),
@dec_total decimal(10,2),
@dec_tax_amt decimal(10,2),
@bit_is_editable bit,
@date_pay_date date,
@str_custome_charge_desc nvarchar(150),
@dec_prev_peack_energy decimal(10,1),
@dec_prev_inter_energy decimal(10,1),
@dec_current_peack_energy decimal(10,1),
@dec_current_inter_energy decimal(10,1)		
)
AS
BEGIN

declare @cnt int=0

	select @cnt= COUNT(*) from tbl_invoice where 
	((@date_s_bill_date>=date_s_bill_date and @date_s_bill_date<=date_e_bill_date) or
	(@date_e_bill_date>=date_s_bill_date and @date_e_bill_date<=date_e_bill_date)) and int_tenant_id=@int_tenant_id


if(@cnt=0)
begin
	insert into tbl_invoice values
	(
	GETDATE(),
	@int_meter_id, 
	@int_tenant_id,
	@bit_tenant_active,
	@date_s_bill_date,
	@date_e_bill_date,
	@dec_peak_energy,@dec_peak_energy_rate,@dec_inter_energy,@dec_inter_energy_rate,
	@dec_custome_charges,
	@dec_demad,@dec_total,null,@dec_tax_amt,@bit_is_editable,
	@date_pay_date,@str_custome_charge_desc,@dec_prev_peack_energy,
	@dec_prev_inter_energy,@dec_current_peack_energy,@dec_current_inter_energy

	)

	select @int_id=@@IDENTITY

	update tbl_user_master set dec_last_peak_energy=@dec_peak_energy,dec_last_inter_energy=@dec_inter_energy where int_id=@int_tenant_id
end
else
	select @int_id=-2
END

GO
/****** Object:  StoredProcedure [dbo].[usp_insert_pm_billing_hours]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_insert_pm_billing_hours]
(
@int_pm_id int,
@str_peak_s_time nvarchar(20),
@str_peak_e_time nvarchar(20),
@str_inter_s_time nvarchar(20),
@str_inter_e_time nvarchar(20)
)
AS
BEGIN
	insert into tbl_pm_billing_hours values(@int_pm_id,@str_peak_s_time,@str_peak_e_time,@str_inter_s_time,@str_inter_e_time)
	

	select 1
END

GO
/****** Object:  StoredProcedure [dbo].[usp_insert_tbl_tenant_email]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_tbl_tenant_email] 
(
@int_tenant_id int,
@str_from_email nvarchar(150),
@str_cc_email nvarchar(150),
@str_bcc_email nvarchar(150),
@str_subject nvarchar(150),
@str_body nvarchar(max)
)
AS
BEGIN
	insert into tbl_tenant_email_setup values(
	@int_tenant_id,@str_from_email,@str_cc_email,@str_bcc_email,@str_subject,@str_body
)
END

GO
/****** Object:  StoredProcedure [dbo].[usp_insert_tenant_billing_info]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_tenant_billing_info]
(
@int_tenant_id int,
@int_template_id int,
@dec_rate decimal(7,2),
@bit_is_seasonal_rate bit,
@dec_seasonal_multi_rate decimal(7,2),
@bit_is_surchare bit,
@dec_surcharge_amt decimal(7,2),
@str_min_billable_over nvarchar(10),
@str_charge_tenant_min nvarchar(10),
@str_charge_tenant_max nvarchar(10),
@bit_is_consolidate_zone bit,
@bit_is_print bit,
@bit_is_file bit,
@int_type int,
@str_email nvarchar(150)
)	
AS
BEGIN
	insert into tbl_tenant_billing_info values
	(
	@int_tenant_id ,
@int_template_id ,
@dec_rate,
@bit_is_seasonal_rate,
@dec_seasonal_multi_rate,
@bit_is_surchare,
@dec_surcharge_amt,
@str_min_billable_over ,
@str_charge_tenant_min ,
@str_charge_tenant_max ,
@bit_is_consolidate_zone ,
@bit_is_print,
@bit_is_file ,
@int_type ,
@str_email

	)

	select @@IDENTITY
END

GO
/****** Object:  StoredProcedure [dbo].[usp_insert_tenant_contract]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_tenant_contract]
(
@int_tenant_id int,
@s_date date,
@e_date date
)
AS
BEGIN
	insert into tbl_tenant_contract values(@int_tenant_id,@s_date,@e_date)
	select @@IDENTITY
END

GO
/****** Object:  StoredProcedure [dbo].[usp_insert_tenant_meter]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Purav Topiwala
-- Create date: 5-july-2017
-- Description:	insert user
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_tenant_meter]
(

@int_tenant_id int,
@int_meter_id int,
@int_id int OUT

)
AS
BEGIN
	
	declare @cnt int

	select @cnt=COUNT(int_tenant_id) from tbl_tenant_meter where int_meter_id=@int_meter_id and (bit_is_assign='True')
	
	if(@cnt=0)
	begin
		insert into tbl_tenant_meter values(@int_tenant_id,@int_meter_id,GETDATE(),null,'True')
		select @int_id=1
	end
	else
		select @int_id=-1
	
END

GO
/****** Object:  StoredProcedure [dbo].[usp_insert_user]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Purav Topiwala
-- Create date: 5-july-2017
-- Description:	insert user
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_user]
(

@str_comp_name nvarchar(150),
@str_contact_name nvarchar(150),
@str_add_1  nvarchar(150)='',
@str_add_2  nvarchar(150)='',
@str_city  nvarchar(100)='',
@str_state nvarchar(50)='',
@int_pin_code int=0,
@str_country nvarchar(50)='',
@str_email nvarchar(150),
@int_user_type_id int,
@str_user_name nvarchar(150),
@str_password nvarchar(20),
@int_pm_id int=0,
@int_user_id int OUTPUT,
@int_invoice_period int=0
)
AS
BEGIN
	
	

	if not exists( select * from tbl_user_master where str_user_name=@str_user_name)
	begin
		insert into tbl_user_master values(@str_comp_name,@str_contact_name,@str_add_1,@str_add_2,@str_city,
		@str_state,@int_pin_code,@str_country,@str_email,@int_user_type_id,@str_user_name,@str_password,@int_pm_id,GETDATE(),0,0,@int_invoice_period)

		select @int_user_id=@@IDENTITY
	end 
	else
		select @int_user_id=-1


	print @int_user_id


END

GO
/****** Object:  StoredProcedure [dbo].[usp_test]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_test] 
(
@sdate date,
@edate date,
@int_tenant_id int
)
AS
BEGIN

declare @cnt int



	select @cnt= COUNT(*) from tbl_invoice where 
	((@sdate>=date_s_bill_date and @sdate<=date_e_bill_date) or
	(@edate>=date_s_bill_date and @edate<=date_e_bill_date)) and int_tenant_id=@int_tenant_id

	select @cnt


END

GO
/****** Object:  StoredProcedure [dbo].[usp_update_bill_rate]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_bill_rate]
(
@int_id int,
@dec_rate decimal(7,2),
@dec_seasonal_rate decimal(7,2),
@dec_surcharge_amt decimal(7,2)
)
AS
BEGIN
	update tbl_tenant_billing_info set 
	dec_rate=@dec_rate,
	dec_seasonal_multi_rate=@dec_seasonal_rate,
	dec_surcharge_amt=@dec_surcharge_amt
	where int_tenant_id=@int_id

	select 1
END

GO
/****** Object:  StoredProcedure [dbo].[usp_update_invoice]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_invoice]
(
@int_id int,
@bit_tenant_active bit,
@date_s_bill_date date,
@date_e_bill_date date,
@dec_peak_energy decimal(10,2),
@dec_peak_energy_rate decimal(10,2),
@dec_inter_energy decimal(10,2),
@dec_inter_energy_rate decimal(10,2),
@dec_custome_charges decimal(10,2),
@dec_demad decimal(10,2),
@dec_total decimal(10,2),
@dec_tax_amt decimal(10,2),
@date_pay_date date,
@str_custome_charge_desc nvarchar(150),
@dec_prev_peack_energy decimal(10,1)=0,
@dec_prev_inter_energy decimal(10,1)=0,
@dec_current_peack_energy decimal(10,1)=0,
@dec_current_inter_energy decimal(10,1)=0,
@int_tenant_id int		
	
)
AS
BEGIN
	update tbl_invoice set
	
	bit_tenant_active=@bit_tenant_active,
	date_s_bill_date=@date_s_bill_date,
	date_e_bill_date=@date_e_bill_date,
	dec_peak_energy=@dec_peak_energy,
	dec_peak_energy_rate=@dec_peak_energy_rate,
	dec_inter_energy=@dec_inter_energy,
	dec_inter_energy_rate=@dec_inter_energy_rate,
	dec_custome_charges=@dec_custome_charges,
	dec_demad=@dec_demad,
	dec_total=@dec_total,
	dec_tax_amt=@dec_tax_amt,
	date_modify=GETDATE(),
	date_pay_date=@date_pay_date,
	str_custome_charge_desc=@str_custome_charge_desc,dec_prev_peack_energy=@dec_prev_peack_energy,
    dec_prev_inter_energy=@dec_prev_inter_energy,
	dec_current_peack_energy=@dec_current_peack_energy,dec_current_inter_energy=@dec_current_inter_energy
	where int_invoice_id=@int_id

	update tbl_user_master set dec_last_peak_energy=@dec_peak_energy,dec_last_inter_energy=@dec_inter_energy where int_id=@int_tenant_id

END

GO
/****** Object:  StoredProcedure [dbo].[usp_update_pm_billing_hours]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_update_pm_billing_hours]
(
@int_pm_id int,
@str_peak_s_time nvarchar(20),
@str_peak_e_time nvarchar(20),
@str_inter_s_time nvarchar(20),
@str_inter_e_time nvarchar(20)
)
AS
BEGIN
	update tbl_pm_billing_hours set 
	str_peak_s_time=@str_peak_s_time,
	str_peak_e_time=@str_peak_e_time,
	str_inter_s_time=@str_inter_s_time,
	str_inter_e_time=@str_inter_e_time
	where int_pm_id=@int_pm_id

	select 1
END
GO
/****** Object:  StoredProcedure [dbo].[usp_update_tbl_tenant_email]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_update_tbl_tenant_email] 
(
@int_tenant_id int,
@str_from_email nvarchar(150),
@str_cc_email nvarchar(150),
@str_bcc_email nvarchar(150),
@str_subject nvarchar(150),
@str_body nvarchar(max),
@int_email_id int
)
AS
BEGIN
	update tbl_tenant_email_setup set
	str_from_email=@str_from_email,
	str_cc_email=@str_cc_email,
	str_bcc_email=@str_bcc_email,
	str_subject=@str_subject,
	str_body=@str_body
	where int_email_id=@int_email_id

END

GO
/****** Object:  StoredProcedure [dbo].[usp_update_tenant_billing_info]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_tenant_billing_info]
(
@int_tenant_id int,
@int_template_id int,
@dec_rate decimal(7,2),
@bit_is_seasonal_rate bit,
@dec_seasonal_multi_rate decimal(7,2),
@bit_is_surchare bit,
@dec_surcharge_amt decimal(7,2),
@str_min_billable_over nvarchar(10),
@str_charge_tenant_min nvarchar(10),
@str_charge_tenant_max nvarchar(10),
@bit_is_consolidate_zone bit,
@bit_is_print bit,
@bit_is_file bit,
@int_type int,
@str_email nvarchar(150),
@int_id int
)	
AS
BEGIN
	update tbl_tenant_billing_info set
	
	
   int_template_id=@int_template_id ,
   dec_rate=@dec_rate,
   bit_is_seasonal_rate=@bit_is_seasonal_rate,
   dec_seasonal_multi_rate=@dec_seasonal_multi_rate,
   bit_is_surchare= @bit_is_surchare,
   dec_surcharge_amt=@dec_surcharge_amt,
   str_min_billable_over=@str_min_billable_over ,
   str_charge_tenant_min=@str_charge_tenant_min ,
   str_charge_tenant_max= @str_charge_tenant_max ,
   bit_is_consolidate_zone= @bit_is_consolidate_zone ,
   bit_is_print=@bit_is_print,
   bit_is_file= @bit_is_file ,
   int_type=@int_type ,
   str_email= @str_email
   where int_id=@int_id
	

	select @@IDENTITY
END

GO
/****** Object:  StoredProcedure [dbo].[usp_update_tenant_contract]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_tenant_contract] 
(
@int_contract_id int,
@s_date date,
@e_date date
)	
AS
BEGIN
	
	update tbl_tenant_contract set
	s_date=@s_date,
	e_date =@e_date
	where int_contract_id=@int_contract_id

	select 1


END

GO
/****** Object:  StoredProcedure [dbo].[usp_update_tenant_meter]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Purav Topiwala
-- Create date: 5-july-2017
-- Description:	insert user
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_tenant_meter]
(
@int_id int,
@int_tenant_id int,
@int_meter_id int,
@int_val int out

)
AS
BEGIN
	
	declare @cnt int

	select @cnt=COUNT(int_tenant_id) from tbl_tenant_meter where int_meter_id=@int_meter_id and (bit_is_assign='True') and int_id<>@int_id
	
	if(@cnt=0)
	begin

		update tbl_tenant_meter set int_tenant_id=@int_tenant_id,
		int_meter_id=@int_meter_id
		where int_id=@int_id
		select @int_val=1

	end
	else
		select @int_val=-1

	
END

GO
/****** Object:  StoredProcedure [dbo].[usp_update_user]    Script Date: 10/27/2017 1:48:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Purav Topiwala
-- Create date: 5-july-2017
-- Description:	insert user
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_user]
(
@int_id int,
@str_comp_name nvarchar(150),
@str_contact_name nvarchar(150),
@str_add_1  nvarchar(150)='',
@str_add_2  nvarchar(150)='',
@str_city  nvarchar(100)='',
@str_state nvarchar(50)='',
@int_pin_code int=0,
@str_country nvarchar(50)='',
@str_email nvarchar(150),
@int_user_type_id int,
@str_user_name nvarchar(150),
@str_password nvarchar(20),
@int_pm_id int=0,
@int_invoice_period int=0
)
AS
BEGIN
	
	if not exists( select * from tbl_user_master where str_user_name=@str_user_name and int_id<>@int_id)
	begin
		update tbl_user_master set  str_comp_name=@str_comp_name,
		str_contact_name=@str_contact_name,
		str_add_1=@str_add_1,
		str_add_2=@str_add_2,
		str_city=@str_city,
		str_state=@str_state,
		int_pin_code=@int_pin_code,
		str_country=@str_country,
		str_email=@str_email,
		str_user_name=@str_user_name,
		str_password=@str_password,
		int_pm_id=@int_pm_id,
		int_invoice_period=@int_invoice_period
		where int_id=@int_id
	end 
	else
		select 1


END

GO
USE [master]
GO
ALTER DATABASE [DB_TenantMNG] SET  READ_WRITE 
GO
