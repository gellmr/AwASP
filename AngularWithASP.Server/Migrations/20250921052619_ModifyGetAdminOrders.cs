using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularWithASP.Server.Migrations
{
  public partial class ModifyGetAdminOrders : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      // Update the stored procedure GetAdminOrders. New version allows searching by various fields.
      migrationBuilder.Sql(@"
        ALTER PROCEDURE [dbo].GetAdminOrders
          @PageNumber INT, @PageSize INT, @BacklogSearch NVARCHAR(250) = NULL
         ,@DateMatchFormat NVARCHAR(50) = NULL
        AS
        BEGIN
          DECLARE @skip INT = (@PageNumber - 1) * @PageSize;
          DECLARE @Search NVARCHAR(252);
          SET @Search = '%' + @BacklogSearch + '%';

          -- Now, execute the main query.
          SELECT
            CAST(ROW_NUMBER() OVER (ORDER BY ord.OrderPlacedDate DESC, ord.UserID ASC, ord.ID DESC) AS INT) AS RowNumber,
            ord.ID as 'OrderID',
            CASE WHEN usr.UserName IS NOT NULL THEN (usr.UserName) ELSE (guest.FirstName + ' ' + guest.LastName) END AS 'Username',
            CASE WHEN usr.Id IS NOT NULL THEN (usr.Id) END AS 'UserID',
            CASE WHEN guest.ID IS NOT NULL THEN convert(nvarchar(50), guest.ID) END AS 'GuestID',
            CASE WHEN guest.ID IS NOT NULL THEN 'Guest' ELSE 'User' END AS 'AccountType',
            CASE WHEN guest.ID IS NOT NULL THEN (guest.Email) ELSE (usr.Email) END AS 'Email',
            ord.[OrderPlacedDate] as 'OrderPlaced',
            CASE WHEN ordWPay.PaymentReceived IS NULL THEN 0 ELSE ordWPay.PaymentReceived END AS 'PaymentReceived',
            CASE WHEN ordWPay.PaymentReceived IS NULL THEN rpay.InvoiceTot ELSE (rpay.InvoiceTot - ordWPay.PaymentReceived) END AS 'Outstanding',
            CASE WHEN opQty.ItemsOrdered IS NULL THEN 0 ELSE opQty.ItemsOrdered END AS 'ItemsOrdered',
            ispTitles.Items as Items,
            ord.OrderStatus,
            CAST(COUNT(*) OVER() AS INT) AS TotalRows
          FROM [dbo].Orders as ord
          LEFT JOIN (
            -- {ord#, ItemsOrdered} Aggregated ordered product quantity.
            --  1     322        
            --  2     21        
            SELECT ord.ID as 'ord#', SUM(op.Quantity) AS 'ItemsOrdered' FROM [dbo].Orders ord LEFT JOIN [dbo].OrderedProducts op on ord.ID = op.OrderID GROUP BY ord.ID
          ) as opQty on ord.ID = opQty.[ord#]
          LEFT JOIN (
            -- {ord#, InvoiceTot} Aggregated price and quantity.
            --  1     5153        
            --  2     1052        
            SELECT ord.ID as 'ord#', SUM(isp.Price * op.Quantity) as 'InvoiceTot'--
            FROM [dbo].Orders as ord
            LEFT JOIN [dbo].OrderedProducts op on ord.ID = op.OrderID
            LEFT JOIN [dbo].InStockProducts isp on isp.ID = op.InStockProductID
            GROUP BY ord.ID
          ) as rpay on ord.ID = rpay.[ord#]
          LEFT JOIN (
            -- {OrderID, PaymentReceived} Aggregated payment values for orders with payments received.
            --  1        3000             
            --  2         552             
            SELECT mto.ID as 'OrderID', SUM(sub.[pay.Amount]) as 'PaymentReceived'
            FROM [dbo].Orders as mto
            INNER JOIN (
              -- {OrderID, pay.ID, pay.Amount} Unaggregated payment data.
              --  2        1       250         
              --  2        1       250         
              --  2        1        52         
              SELECT ord.ID as 'OrderID', pay.ID AS 'pay.ID', pay.Amount as 'pay.Amount'
              FROM [dbo].[OrderPayments] as pay
            LEFT JOIN [dbo].Orders as ord on pay.OrderID = ord.ID
              GROUP BY ord.ID, pay.ID, pay.Amount
            )
            as sub ON mto.ID = sub.OrderID
            GROUP BY mto.ID
          ) AS ordWPay ON ord.ID = ordWPay.OrderID
          LEFT JOIN [dbo].[AspNetUsers] AS usr ON ord.UserID = usr.Id
          LEFT JOIN [dbo].[Guests] AS guest ON ord.GuestID = guest.ID
          LEFT JOIN(
            -- {ord#, Items} String aggregated product titles, for each order. Eg 'Speed Chess Timer, Thinking Cap, Hydralite, Soccer Ball'
            SELECT ord.ID as 'ord#', STRING_AGG ( isp.Title, ', ' ) as 'Items' FROM [dbo].Orders ord
            LEFT JOIN [dbo].OrderedProducts op on ord.ID = op.OrderID LEFT JOIN [dbo].InStockProducts isp on isp.ID = op.InStockProductID
            GROUP BY ord.ID
          ) As ispTitles ON ispTitles.[ord#] = ord.ID
          WHERE @BacklogSearch IS NULL OR (
            CAST(ord.ID AS NVARCHAR(10)) LIKE @Search
            OR (CASE WHEN usr.UserName IS NOT NULL THEN (usr.UserName) ELSE (guest.FirstName + ' ' + guest.LastName) END) LIKE @Search
            OR CAST((usr.Id) AS NVARCHAR(50)) LIKE @Search
	          OR CAST((guest.ID) AS NVARCHAR(50)) LIKE @Search
	          OR (CASE WHEN guest.ID IS NOT NULL THEN 'Guest' ELSE 'User' END) LIKE @Search
	          OR (CASE WHEN guest.ID IS NOT NULL THEN (guest.Email) ELSE (usr.Email) END) LIKE @Search
            OR (@DateMatchFormat IS NOT NULL AND FORMAT(ord.[OrderPlacedDate], @DateMatchFormat) LIKE @Search)
            OR CAST((CASE WHEN ordWPay.PaymentReceived IS NULL THEN 0 ELSE ordWPay.PaymentReceived END) AS NVARCHAR(50)) LIKE @Search
            OR CAST((CASE WHEN ordWPay.PaymentReceived IS NULL THEN rpay.InvoiceTot ELSE (rpay.InvoiceTot - ordWPay.PaymentReceived) END) AS NVARCHAR(50)) LIKE @Search
	          OR CAST((opQty.ItemsOrdered) AS NVARCHAR(50)) LIKE @Search
	          OR (ispTitles.Items) LIKE @Search
	          OR (ord.OrderStatus) LIKE @Search
          )
          ORDER BY ord.OrderPlacedDate DESC, ord.UserID ASC, ord.ID DESC
          OFFSET @skip ROWS
          FETCH NEXT @PageSize ROWS ONLY;

        END;
      ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      // Restore GetAdminOrders according to 20250902082954_CreateSPGetAdminOrders
      migrationBuilder.Sql(@"
        ALTER PROCEDURE GetAdminOrders
          @PageNumber INT, @PageSize INT
        AS
        BEGIN
          DECLARE @skip INT = (@PageNumber - 1) * @PageSize;
          SELECT ord.ID as 'OrderID',
            CASE WHEN usr.UserName IS NOT NULL THEN (usr.UserName) ELSE (guest.FirstName + ' ' + guest.LastName) END AS 'Username',
            CASE WHEN usr.Id IS NOT NULL THEN (usr.Id) END AS 'UserID',
            CASE WHEN guest.ID IS NOT NULL THEN convert(nvarchar(50), guest.ID) END AS 'GuestID',
            CASE WHEN guest.ID IS NOT NULL THEN 'Guest' ELSE 'User' END AS 'AccountType',
            CASE WHEN guest.ID IS NOT NULL THEN (guest.Email) ELSE (usr.Email) END AS 'Email',
            ord.[OrderPlacedDate] as 'OrderPlaced',
            CASE WHEN ordWPay.PaymentReceived IS NULL THEN 0 ELSE ordWPay.PaymentReceived END AS 'PaymentReceived',
            CASE WHEN ordWPay.PaymentReceived IS NULL THEN rpay.InvoiceTot ELSE (rpay.InvoiceTot - ordWPay.PaymentReceived) END AS 'Outstanding',
            CASE WHEN opQty.ItemsOrdered IS NULL THEN 0 ELSE opQty.ItemsOrdered END AS 'ItemsOrdered',
            ispTitles.Items as Items,
            ord.OrderStatus
          FROM [dbo].Orders as ord

          LEFT JOIN (
            -- {ord#, ItemsOrdered} Aggregated op quantity for all 70 orders.
            --  1     322        
            --  2     21        
            SELECT ord.ID as 'ord#', SUM(op.Quantity) AS 'ItemsOrdered' FROM [dbo].Orders ord LEFT JOIN [dbo].OrderedProducts op on ord.ID = op.OrderID GROUP BY ord.ID
          ) as opQty on ord.ID = opQty.[ord#]

          LEFT JOIN (
            -- {ord#, InvoiceTot} Aggregated price and quantity for all 70 orders.
            --  1     5153        
            --  2     1052        
            SELECT ord.ID as 'ord#', SUM(isp.Price * op.Quantity) as 'InvoiceTot'--
            FROM [dbo].Orders as ord
            LEFT JOIN [dbo].OrderedProducts op on ord.ID = op.OrderID
            LEFT JOIN [dbo].InStockProducts isp on isp.ID = op.InStockProductID
            GROUP BY ord.ID
          ) as rpay on ord.ID = rpay.[ord#]

          LEFT JOIN (
            -- {OrderID, PaymentReceived} Aggregated payment values, for orders with payments received.
            --  1        3000             
            --  2         552             
            SELECT mto.ID as 'OrderID', SUM(sub.[pay.Amount]) as 'PaymentReceived'
            FROM [dbo].Orders as mto
            INNER JOIN (
              -- {OrderID, pay.ID, pay.Amount} Unaggregated payment data.
              --  2        1       250         
              --  2        1       250         
              --  2        1        52         
              SELECT ord.ID as 'OrderID', pay.ID AS 'pay.ID', pay.Amount as 'pay.Amount'
              FROM [dbo].[OrderPayments] as pay
            LEFT JOIN [dbo].Orders as ord on pay.OrderID = ord.ID
              GROUP BY ord.ID, pay.ID, pay.Amount
            )
            as sub ON mto.ID = sub.OrderID
            GROUP BY mto.ID
          ) AS ordWPay ON ord.ID = ordWPay.OrderID

          LEFT JOIN [dbo].[AspNetUsers] AS usr ON ord.UserID = usr.Id

          LEFT JOIN [dbo].[Guests] AS guest ON ord.GuestID = guest.ID

          LEFT JOIN(
            -- {ord#, Items} String aggregated product titles, for each order.
            --  1    ""Life Jacket, Camping Towel, Waterproof Equipment Bag ...""
            --  2    ""Speed Chess Timer, Thinking Cap, Hydralite, Soccer Ball...""
            SELECT ord.ID as 'ord#', STRING_AGG ( isp.Title, ', ' ) as 'Items' FROM [dbo].Orders ord
            LEFT JOIN [dbo].OrderedProducts op on ord.ID = op.OrderID LEFT JOIN [dbo].InStockProducts isp on isp.ID = op.InStockProductID
            GROUP BY ord.ID
          ) As ispTitles ON ispTitles.[ord#] = ord.ID

          ORDER BY ord.OrderPlacedDate DESC, ord.UserID ASC, ord.ID DESC

          OFFSET @skip ROWS
          FETCH NEXT @PageSize ROWS ONLY;
        END;
      ");
    }
  }
}

