﻿insert into SyncDBLogs(LogText) values('Parameter passed for @TargetDate: ' + convert(varchar, convert(datetime, @TargetDate), 101) + ' @arg2: ' + @arg2);