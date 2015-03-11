INSERT [dbo].[PageType] ([PageTypeId], [ParentPageTypeId], [Name], [IsFolder], [ControllerType], [ActionName], [ModelType]) VALUES (N'c55d8fac-832a-414e-b59f-068b0b3a48b6', N'15a401b7-c534-41a7-b42b-cc0c85458fb5', N'Page test', 0, N'Kardarak.Controllers.StandardPageController', N'Test', N'StandardPageModel')
GO
INSERT [dbo].[PageType] ([PageTypeId], [ParentPageTypeId], [Name], [IsFolder], [ControllerType], [ActionName], [ModelType]) VALUES (N'a1d27a9a-3e12-4752-bd47-bca64f105dba', N'15a401b7-c534-41a7-b42b-cc0c85458fb5', N'Page - Afficher', 0, N'Kardarak.Controllers.StandardPageController', N'Show', N'StandardPageModel')
GO
INSERT [dbo].[PageType] ([PageTypeId], [ParentPageTypeId], [Name], [IsFolder], [ControllerType], [ActionName], [ModelType]) VALUES (N'15a401b7-c534-41a7-b42b-cc0c85458fb5', NULL, N'Pages simples', 1, NULL, NULL, NULL)
GO
INSERT [dbo].[Page] ([PageId], [ParentPageId], [PageTypeId]) VALUES (N'3b8706bb-0ba7-430e-be93-20475e7359c9', N'51e9e402-f56a-4c27-b6a4-608e6040a521', N'a1d27a9a-3e12-4752-bd47-bca64f105dba')
GO
INSERT [dbo].[Page] ([PageId], [ParentPageId], [PageTypeId]) VALUES (N'51e9e402-f56a-4c27-b6a4-608e6040a521', NULL, N'a1d27a9a-3e12-4752-bd47-bca64f105dba')
GO
INSERT [dbo].[Page] ([PageId], [ParentPageId], [PageTypeId]) VALUES (N'b9ba3681-ff6c-4895-bc82-a71c62cb59de', N'51e9e402-f56a-4c27-b6a4-608e6040a521', N'c55d8fac-832a-414e-b59f-068b0b3a48b6')
GO
INSERT [dbo].[PageLanguage] ([PageId], [Language], [NameUrl], [CopyFromLanguage], [ContentJson]) VALUES (N'3b8706bb-0ba7-430e-be93-20475e7359c9', N'en', N'Page1', NULL, N'{"Navigation":{"PageTitle":"Page 1","IncludeInNavigation":true,"NavigationTitle":"Page 1"},"PageContent":"<p>Page 1</p>"}')
GO
INSERT [dbo].[PageLanguage] ([PageId], [Language], [NameUrl], [CopyFromLanguage], [ContentJson]) VALUES (N'3b8706bb-0ba7-430e-be93-20475e7359c9', N'fr-CA', N'Page1', NULL, N'{"Navigation":{"PageTitle":"Page 1","IncludeInNavigation":true,"NavigationTitle":"Page 1"},"PageContent":"<p>Page 1</p>"}')
GO
INSERT [dbo].[PageLanguage] ([PageId], [Language], [NameUrl], [CopyFromLanguage], [ContentJson]) VALUES (N'51e9e402-f56a-4c27-b6a4-608e6040a521', N'en', N'Home', NULL, N'{"Navigation":{"PageTitle":"Home","IncludeInNavigation":true,"NavigationTitle":"Home"},"PageContent":"<p>Hello world!</p>"}')
GO
INSERT [dbo].[PageLanguage] ([PageId], [Language], [NameUrl], [CopyFromLanguage], [ContentJson]) VALUES (N'51e9e402-f56a-4c27-b6a4-608e6040a521', N'fr-CA', N'Accueil', NULL, N'{"Navigation":{"PageTitle":"Accueil","IncludeInNavigation":true,"NavigationTitle":"Accueil"},"PageContent":"<p>Bonjours les amis</p>"}')
GO
INSERT [dbo].[PageLanguage] ([PageId], [Language], [NameUrl], [CopyFromLanguage], [ContentJson]) VALUES (N'b9ba3681-ff6c-4895-bc82-a71c62cb59de', N'en', N'Test', NULL, N'{"Navigation":{"PageTitle":"Test","IncludeInNavigation":true,"NavigationTitle":"Test"},"PageContent":"<p>Test</p>"}')
GO
INSERT [dbo].[PageLanguage] ([PageId], [Language], [NameUrl], [CopyFromLanguage], [ContentJson]) VALUES (N'b9ba3681-ff6c-4895-bc82-a71c62cb59de', N'fr-CA', N'Test', NULL, N'{"Navigation":{"PageTitle":"Test","IncludeInNavigation":true,"NavigationTitle":"Test"},"PageContent":"<p>Test</p>"}')
GO
