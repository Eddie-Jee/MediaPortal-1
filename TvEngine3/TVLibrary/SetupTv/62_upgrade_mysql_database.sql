USE %TvLibrary%;

ALTER TABLE mptvdb.channelgroup ADD COLUMN pinCode VARCHAR(5) NULL  AFTER sortOrder ;

UPDATE version SET versionNumber=62 WHERE idVersion=1;