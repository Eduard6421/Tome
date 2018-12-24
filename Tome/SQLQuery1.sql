delete from TomeDB.dbo.CurrentVersions;
delete from TomeDB.dbo.TomeHistories;
delete from TomeDB.dbo.Tomes;

select * from TomeDB.dbo.CurrentVersions;
select * from TomeDB.dbo.TomeHistories;
select * from TomeDB.dbo.Tomes;
select * from TomeDB.dbo.Tags;
select * from TomeDB.dbo.TagReferenes;

select * from TomeDB.dbo.CurrentVersions
where CurrentVersions.TomeId = 19;