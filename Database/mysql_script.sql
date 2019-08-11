
CREATE TABLE `user`(
`Id` varchar(32) not null PRIMARY KEY,
`CreateTime` DATETIME NOT NULL,
`CreateBy` varchar(32) not null,
`ModifyTime` DATETIME NOT NULL,
`ModifyBy` varchar(32) not null,
`Name` varchar(50) not NULL,
`Password` varchar(1024) not null,
`Mobilephone` varchar(11) not null,
`Email` varchar(50) not null,
`Gender` int not null,
`UserType` int not null default 0
);

CREATE TABLE `user_profile`(
`Id` varchar(32) not null PRIMARY KEY,
`CreateTime` DATETIME NOT NULL,
`CreateBy` varchar(32) not null,
`ModifyTime` DATETIME NOT NULL,
`ModifyBy` varchar(32) not null,
`Name` varchar(50) not NULL,
`Mobilephone` varchar(11) not null,
`Email` varchar(50) not null,
`Gender` int not null,
`HeadImg` varchar(32) not null
)

create table `role`(
`Id` varchar(32) not null PRIMARY KEY,
`CreateTime` DATETIME NOT NULL,
`CreateBy` varchar(32) not null,
`ModifyTime` DATETIME NOT NULL,
`ModifyBy` varchar(32) not null,
`Name` VARCHAR(50) not null,
`DisplayName` VARCHAR(50) not NULL
);

create table `user_role`(
`Id` varchar(32) not null PRIMARY KEY,
`CreateTime` DATETIME NOT NULL,
`CreateBy` varchar(32) not null,
`ModifyTime` DATETIME NOT NULL,
`ModifyBy` varchar(32) not null,
`UserId` VARCHAR(32) not null,
`RoleId` VARCHAR(32) not NULL
);

create table `refresh_token`(
`Id` int not null PRIMARY KEY AUTO_INCREMENT,
`CreateTime` DATETIME NOT NULL,
`CreateBy` varchar(32) not null,
`Token` varchar(32) not null,
`UserId` varchar(32) not null,
`ExpireTime` datetime not null
);

create table `app_client`(
`Id` varchar(32) not null PRIMARY KEY,
`CreateTime` DATETIME NOT NULL,
`CreateBy` varchar(32) not null,
`ModifyTime` DATETIME NOT NULL,
`ModifyBy` varchar(32) not null,
`ClientId` varchar(32) not null,
`ClientSecret` varchar(32) not null,
`ReturnUrl` varchar(1024) null
);

CREATE TABLE `log` (
  `Id` int not null PRIMARY KEY AUTO_INCREMENT,
  `Application` varchar(50) DEFAULT NULL,
  `Logged` datetime DEFAULT NULL,
  `Level` varchar(50) DEFAULT NULL,
  `Message` varchar(512) DEFAULT NULL,
  `Logger` varchar(250) DEFAULT NULL,
  `Callsite` varchar(1024) DEFAULT NULL,
  `Exception` text DEFAULT NULL
)