/****** Object:  Schema [Throttler]    Script Date: 11/3/2016 5:27:54 PM ******/
CREATE SCHEMA [Throttler]
GO
/****** Object:  Table [Throttler].[Configuration]    Script Date: 11/3/2016 5:27:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Throttler].[Configuration](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](50) NOT NULL,
	[IpThrottling] [bit] NOT NULL,
	[ClientThrottling] [bit] NOT NULL,
	[EndpointThrottling] [bit] NOT NULL,
	[StackBlockedRequests] [bit] NOT NULL,
	[LimitPerSecond] [bigint] NOT NULL,
	[LimitPerMinute] [bigint] NOT NULL,
	[LimitPerHour] [bigint] NOT NULL,
	[LimitPerDay] [bigint] NOT NULL,
	[LimitPerWeek] [bigint] NOT NULL,
 CONSTRAINT [PK_Configuration_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Throttler].[PolicyRule]    Script Date: 11/3/2016 5:27:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Throttler].[PolicyRule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConfigurationId] [int] NOT NULL,
	[PolicyTypeId] [int] NOT NULL,
	[Entry] [nvarchar](50) NOT NULL,
	[PerSecond] [bigint] NULL,
	[PerMinute] [bigint] NULL,
	[PerHour] [bigint] NULL,
	[PerDay] [bigint] NULL,
	[PerWeek] [bigint] NULL,
 CONSTRAINT [PK_PolicyRule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Throttler].[PolicyType]    Script Date: 11/3/2016 5:27:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Throttler].[PolicyType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_PolicyType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Throttler].[RequestCounter]    Script Date: 11/3/2016 5:27:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Throttler].[RequestCounter](
	[Id] [nchar](50) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[TotalRequests] [bigint] NOT NULL,
	[ExpirationTime] [bigint] NOT NULL,
	[ClientKey] [nchar](25) NOT NULL,
 CONSTRAINT [PK_ThrottleCounter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Throttler].[RequestLog]    Script Date: 11/3/2016 5:27:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Throttler].[RequestLog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestId] [nchar](50) NOT NULL,
	[ClientIp] [nchar](25) NULL,
	[ClientKey] [nchar](50) NULL,
	[Endpoint] [nchar](50) NULL,
	[TotalRequest] [bigint] NOT NULL,
	[StartPeriod] [datetime] NOT NULL,
	[RateLimit] [bigint] NOT NULL,
	[RateLimitPeriod] [nchar](10) NULL,
	[LogDate] [datetime] NOT NULL,
	[Request] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_RequestLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Throttler].[Whitelist]    Script Date: 11/3/2016 5:27:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Throttler].[Whitelist](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConfigurationId] [int] NOT NULL,
	[PolicyTypeId] [int] NOT NULL,
	[Entry] [nvarchar](50) NULL,
 CONSTRAINT [PK_Whitelist] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [Throttler].[Configuration] ON 

GO
INSERT [Throttler].[Configuration] ([Id], [ServerName], [IpThrottling], [ClientThrottling], [EndpointThrottling], [StackBlockedRequests], [LimitPerSecond], [LimitPerMinute], [LimitPerHour], [LimitPerDay], [LimitPerWeek]) VALUES (1, N'localhost', 0, 0, 1, 0, 0, 0, 0, 0, 0)
GO
SET IDENTITY_INSERT [Throttler].[Configuration] OFF
GO
SET IDENTITY_INSERT [Throttler].[PolicyRule] ON 

GO
INSERT [Throttler].[PolicyRule] ([Id], [ConfigurationId], [PolicyTypeId], [Entry], [PerSecond], [PerMinute], [PerHour], [PerDay], [PerWeek]) VALUES (1, 1, 1, N'::1', NULL, NULL, NULL, NULL, NULL)
GO
INSERT [Throttler].[PolicyRule] ([Id], [ConfigurationId], [PolicyTypeId], [Entry], [PerSecond], [PerMinute], [PerHour], [PerDay], [PerWeek]) VALUES (2, 1, 2, N'api-client-key-1', 2, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [Throttler].[PolicyRule] OFF
GO
SET IDENTITY_INSERT [Throttler].[PolicyType] ON 

GO
INSERT [Throttler].[PolicyType] ([Id], [Name]) VALUES (1, N'IpThrottling')
GO
INSERT [Throttler].[PolicyType] ([Id], [Name]) VALUES (2, N'CLientThrottling')
GO
INSERT [Throttler].[PolicyType] ([Id], [Name]) VALUES (3, N'EndpointThrottling')
GO
SET IDENTITY_INSERT [Throttler].[PolicyType] OFF
GO
ALTER TABLE [Throttler].[PolicyRule]  WITH CHECK ADD  CONSTRAINT [FK_PolicyRule_Configuration] FOREIGN KEY([ConfigurationId])
REFERENCES [Throttler].[Configuration] ([Id])
GO
ALTER TABLE [Throttler].[PolicyRule] CHECK CONSTRAINT [FK_PolicyRule_Configuration]
GO
ALTER TABLE [Throttler].[PolicyRule]  WITH CHECK ADD  CONSTRAINT [FK_PolicyRule_PolicyType] FOREIGN KEY([PolicyTypeId])
REFERENCES [Throttler].[PolicyType] ([Id])
GO
ALTER TABLE [Throttler].[PolicyRule] CHECK CONSTRAINT [FK_PolicyRule_PolicyType]
GO
ALTER TABLE [Throttler].[Whitelist]  WITH CHECK ADD  CONSTRAINT [FK_Whitelist_Configuration] FOREIGN KEY([ConfigurationId])
REFERENCES [Throttler].[Configuration] ([Id])
GO
ALTER TABLE [Throttler].[Whitelist] CHECK CONSTRAINT [FK_Whitelist_Configuration]
GO
ALTER TABLE [Throttler].[Whitelist]  WITH CHECK ADD  CONSTRAINT [FK_Whitelist_PolicyType] FOREIGN KEY([PolicyTypeId])
REFERENCES [Throttler].[PolicyType] ([Id])
GO
ALTER TABLE [Throttler].[Whitelist] CHECK CONSTRAINT [FK_Whitelist_PolicyType]
GO
