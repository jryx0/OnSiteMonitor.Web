
delete from [ClueDate_RegionReport];
--地区索数汇总
Insert into [dbo].[ClueDate_RegionReport](RegionGuid, ItemType, ClueType, Clues, Table2) 
SELECT @RegionGuid as Guid,
       table1 AS ItemName,
       cluetype AS ClueType,
       count(*) AS Number,
	   table2
FROM [dbo].[ClueData_HB]
GROUP BY  table1 , table2, 
         cluetype


--市州线索汇总
Insert into [dbo].[ClueDate_Report] (RegionGuid, ItemType, TotalClues, InputErrors)
Select '@RegionGuid' as Guid, a.ItemType , a.totalClues as TotalClues, b.totalClues as InputErrors From
(SELECT 
       ItemType ,       
	   sum(Clues) as totalClues
From [ClueDate_RegionReport]
--Where RegionGuid
group by ItemType) a ,
(SELECT 
       ItemType ,       
	   sum(Clues) as totalClues
From [ClueDate_RegionReport]
Where  substring(table2, 1, 2) = '44' or substring(table2, 1, 2) = '43'
group by ItemType ) b
where a.ItemType = b.ItemType





  
--身份证录入错误数据
(SELECT table1 AS ItemName,   
cluetype AS ClueType,         
       count(*) AS Number 	   
FROM [dbo].[ClueData_HBHGYS]
where substring(table2, 1, 2) = '44' or substring(table2, 1, 2) = '43'
GROUP BY table1, ClueType) b 
	where a.ItemName = b.ItemName and a.ClueType = b.ClueType

--问题数据




--处理数据(人）



		 