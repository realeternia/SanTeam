ConfigDataManager = BaseClass("ConfigDataManager");

function ConfigDataManager:Ctor()
@requireConfigTable
end

function ConfigDataManager:Refresh(language)
@clearConfigTable
end

@getConfigTable()

return ConfigDataManager.New();