
CREATE TABLE user(
`Id` varchar(32) not null PRIMARY KEY,
`CreateTime` DATETIME NOT NULL,
`CreateBy` varchar(32) not null,
`ModifyTime` DATETIME NOT NULL,
`ModifyBy` varchar(32) not null,
`Name` varchar(50) not NULL,
`Password` varchar(1024) not null,
`Mobilephone` varchar(11) not null,
`Email` varchar(50) not null,
`Gender` int not null
);

CREATE TABLE user_profile(
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
