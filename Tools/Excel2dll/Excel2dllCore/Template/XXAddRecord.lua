@xxxConfig = BaseClass("@xxxConfig");

function @xxxConfig:Ctor()
end

function @xxxConfig:Init(@parms)
@fields
end

@xxxConfigTable = BaseClass("@xxxConfigTable");

function @xxxConfigTable:Ctor()
	self:Clear(0);
end

function @xxxConfigTable:Clear(language)
	rawset(self,"DataArray",{});
@keys
	
@alias

	rawset(self, "Size", 0);
	rawset(self,"language",language);
end

function @xxxConfigTable:Init()
	if(self.language == Language.Chs) then
@chsDatas
	elseif(self.language == Language.Cht) then
@chtDatas
	elseif(self.language == Language.Eng) then
@engDatas
	end
@size
end

--添加一条数据
function @xxxConfigTable:AddData(@parms)
	local data = @xxxConfig:New();
	data:Init(@parms);
	
@dataByXxx

	table.insert(self.DataArray,data);
end

function @xxxConfigTable:Check()
	if(self.Size == 0) then
		self:Init();
	end
end

@getDataByXxx()

--所有数据
function @xxxConfigTable:GetDataArray()
	self:Check();
	return self.DataArray;
end

return @xxxConfigTable.New();