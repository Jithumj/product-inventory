USE [ProductInventoryDB]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 17-07-2025 22:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductCode] [nvarchar](50) NOT NULL,
	[ProductName] [nvarchar](200) NOT NULL,
	[ProductImage] [varbinary](max) NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NOT NULL,
	[CreatedUser] [uniqueidentifier] NOT NULL,
	[IsFavourite] [bit] NOT NULL,
	[Active] [bit] NOT NULL,
	[HSNCode] [nvarchar](100) NULL,
	[TotalStock] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ProductCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductVariantCombinationOptions]    Script Date: 17-07-2025 22:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductVariantCombinationOptions](
	[ProductVariantCombinationId] [uniqueidentifier] NOT NULL,
	[VariantId] [uniqueidentifier] NOT NULL,
	[VariantOptionId] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductVariantCombinationId] ASC,
	[VariantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductVariantCombinations]    Script Date: 17-07-2025 22:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductVariantCombinations](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[CombinationCode] [nvarchar](200) NOT NULL,
	[Stock] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 17-07-2025 22:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](200) NOT NULL,
	[Password] [nvarchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VariantOptions]    Script Date: 17-07-2025 22:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VariantOptions](
	[Id] [uniqueidentifier] NOT NULL,
	[VariantId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Variants]    Script Date: 17-07-2025 22:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Variants](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [IsFavourite]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [TotalStock]
GO
ALTER TABLE [dbo].[ProductVariantCombinations] ADD  DEFAULT ((0)) FOR [Stock]
GO
ALTER TABLE [dbo].[ProductVariantCombinationOptions]  WITH CHECK ADD FOREIGN KEY([ProductVariantCombinationId])
REFERENCES [dbo].[ProductVariantCombinations] ([Id])
GO
ALTER TABLE [dbo].[ProductVariantCombinationOptions]  WITH CHECK ADD FOREIGN KEY([VariantId])
REFERENCES [dbo].[Variants] ([Id])
GO
ALTER TABLE [dbo].[ProductVariantCombinationOptions]  WITH CHECK ADD FOREIGN KEY([VariantOptionId])
REFERENCES [dbo].[VariantOptions] ([Id])
GO
ALTER TABLE [dbo].[ProductVariantCombinations]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
GO
ALTER TABLE [dbo].[VariantOptions]  WITH CHECK ADD FOREIGN KEY([VariantId])
REFERENCES [dbo].[Variants] ([Id])
GO
ALTER TABLE [dbo].[Variants]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
GO
CREATE TABLE Logs (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CreatedAt DATETIMEOFFSET NOT NULL,
    LogMessage NVARCHAR(Max) NOT NULL
);
