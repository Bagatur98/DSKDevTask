За задачата се нуждаем от само 2 таблици в базата данни: Кредити и Фактури (Credits and Invoices)

В таблица Credits имаме колони според изискванията : Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                     ClientName TEXT NOT NULL,
                                                     CreditAmount REAL NOT NULL,
                                                     CreditDate TEXT NOT NULL,
                                                     Status INTEGER NOT NULL
                                                     
като Id е ключа на таблицата, 
     CreditAmount е REAL, което съответсва на double в C#, 
     а Status е INTEGER, който съответства на enum CreditStatus, в който има 3 стойности: Created = 0,
                                                                                          AwaitingPayment = 1,
                                                                                          Paid = 2,

В таблица Invoices имаме колони според изискванията : Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                      CreditId INTEGER,
                                                      Amount REAL NOT NULL,
                                                      FOREIGN KEY(CreditId) References Credits(Id) ON DELETE Cascade,

както и колона CreditId, която е foreign key към таблица Credits. Връзката Credits -> Invoices e one-to-many
