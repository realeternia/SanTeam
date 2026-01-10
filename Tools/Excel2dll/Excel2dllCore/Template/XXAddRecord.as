package test
{
	public class @fileNameTable
	{
		private var dataIndex:Vector.<int>;
		private var data:Array;
		private static var _instance: @fileNameTable;
		public static function getInstance(): @fileNameTable
		{
			if (@fileNameTable._instance)
			{
				return @fileNameTable._instance;
			}
			@fileNameTable._instance = new @fileNameTable();
			return @fileNameTable._instance;
		}
		public function @fileNameTable()
		{
			data = new Array();
			init();
		}
		private function init():void
		{
			dataIndex = new Vector.<int>();
			dataIndex.push(
@adddata
			);
		}
		public function getList():Array{
			if(data.length<dataIndex.length)
			{
				for (var key:int in dataIndex) {
					getDataById(key);
				}
			}
			return data;
		}
		public function getDataById(id:Number):@fileName{
			if(this.data[id])
			{
				return this.data[id];
			}
			else
			{
				var config: @fileName = this.getNewDataById(id);
				if(config)
					this.data[id] = config;
				return config; 
			}
		}
		private	function getNewDataById(id:Number): @fileName
		{
			switch(id)
			{
@idCase
			}
			return undefined;
		}
	}
}