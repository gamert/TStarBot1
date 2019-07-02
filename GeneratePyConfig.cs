        static void SaveFile(string filename,string text)
        {
            //string dir = Application.dataPath + "/" + outPath + "/";
            //if (!Directory.Exists(dir))
            //{
            //    Directory.CreateDirectory(dir);
            //}
            System.IO.File.WriteAllText(filename, text);

            Debug.LogWarning("SaveFile :"+ filename);

        }

        //生成PythonAI使用的配置...
        public void GeneratePyConfig()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("from enum import Enum, unique\n");
            sb.AppendLine("@unique");
            sb.AppendLine("class RACE(Enum):");
            sb.AppendLine("        Protoss = 1");
            sb.AppendLine("        Terran = 2");
            sb.AppendLine("        Zerg = 3");


            //UNIT_TYPEID
            sb.AppendLine("@unique");
            sb.AppendLine("class UNIT_TYPEID(Enum):");
            sb.AppendLine("        INVALID = 0");

            var enu = LayoutRecordMgr.HeroCfgDt.GetEnumerator();
            while (enu.MoveNext())
            {
                HeroCfg cfg = enu.Current.Value as HeroCfg;
                sb.AppendLine(string.Format("        UNIT_TYPE{0} = {1},", cfg.Id, cfg.Id));
            }

            enu = LayoutRecordMgr.BuildingCfgDt.GetEnumerator();
            while (enu.MoveNext())
            {
                BuildingCfg cfg = enu.Current.Value as BuildingCfg;
                sb.AppendLine(string.Format("        BUILDING_TYPE{0} = {1},", cfg.Id, cfg.Id));
                //for (int i = 0; i < buildingcfg.iStudyGroup.Length; i++)
                //{
                //    //if (buildingcfg.iStudyGroup[i] == GroupId)
                //    //{
                //    //    return buildingcfg.id;
                //    //}
                //}
            }
            //UNIT_TYPEID
            sb.AppendLine("@unique");
            sb.AppendLine("class UPGRADE_ID(Enum):");
            sb.AppendLine("        INVALID = 0");
            enu = LayoutRecordMgr.BuildingCfgDt.GetEnumerator();
            while (enu.MoveNext())
            {
                BuildingCfg cfg = enu.Current.Value as BuildingCfg;
                if(cfg.iUpgradeBuilding>0)
                {
                    sb.AppendLine(string.Format("        UPGRADE_{0} = {1},", cfg.iUpgradeBuilding, cfg.iUpgradeBuilding));
                }
            }

            SaveFile(Application.dataPath + "/typeenums.py", sb.ToString());

            sb.Remove(0, sb.Length);
            //找到各个种族的农民，用来填充..


            //TypeData (race= 0, mineralCost= 0, gasCost= 0, supplyCost= 0, buildTime= 0, isUnit= False, isBuilding= False, isWorker= False, isRefinery= False, 
            //  isSupplyProvider = False, isResourceDepot= False, isAddon= False, buildAbility= 0, warpAbility= 0, whatBuilds=[], requiredUnits=[], requiredUpgrades=[]):
            //TypeData(RACE.Protoss, 100, 0, 0, 0, True, True, False, False, True, False, False, ABILITY_ID.EFFECT_PHOTONOVERCHARGE.value, 0, [UNIT_TYPEID.PROTOSS_MOTHERSHIPCORE.value, UNIT_TYPEID.PROTOSS_PYLON.value], [], [])
            //建筑:
            enu = LayoutRecordMgr.BuildingCfgDt.GetEnumerator();
            while (enu.MoveNext())
            {
                BuildingCfg cfg = enu.Current.Value as BuildingCfg;
                string typeData = "        TypeData(";
                typeData += cfg.iFamilyType;
                typeData += "," + cfg.iEmployGoldCost;
                typeData += "," + cfg.iEmployWoodCost;
                typeData += "," + cfg.iOccopPopulation;
                typeData += "," + cfg.iEmployTimeCost;
                typeData += "," + "True";               //isUnit
                typeData += "," + "True";               //isBuilding
                typeData += "," + "False";              //isWorker
                typeData += "," + "False";              //isRefinery
                typeData += "," + "False";              //isSupplyProvider
                typeData += "," + "False";              //isResourceDepot
                typeData += "," + "False";              //isAddon
                typeData += "," + "0";                  //buildAbility
                typeData += "," + "0";                  //warpAbility
                typeData += "," + "[]";                  //whatBuilds
                typeData += "," + "[]";                  //requiredUnits
                typeData += "," + "[]";                  //requiredUpgrades
                typeData += ")";
                sb.AppendLine(string.Format("self.m_unitTypeData[{0}] = {1}", cfg.Id, typeData));
            }
            //小兵
            enu = LayoutRecordMgr.HeroCfgDt.GetEnumerator();
            while (enu.MoveNext())
            {
                //UNIT_TYPEID.PROTOSS_PYLONOVERCHARGED.value
                HeroCfg cfg = enu.Current.Value as HeroCfg;
                string typeData = "        TypeData(";
                typeData += cfg.iFamilyType;
                typeData += "," + cfg.iEmployGoldCost;
                typeData += "," + cfg.iEmployWoodCost;
                typeData += "," + cfg.iOccopPopulation;
                typeData += "," + cfg.iEmployTimeCost;  //一般小兵都算建造时间?
                typeData += "," + "True";               //isUnit    默认都是单位
                typeData += "," + "False";              //isBuilding 是否为建筑
                typeData += "," + (cfg.iType == 3? "True":"False");              //isWorker
                typeData += "," + "False";              //isRefinery    精炼厂，类型:重型-机械-建筑
                typeData += "," + "False";              //isSupplyProvider 补给站
                typeData += "," + "False";              //isResourceDepot  资源仓库
                typeData += "," + "False";              //isAddon
                typeData += "," + "0";                  //buildAbility，需要的
                typeData += "," + "0";                  //warpAbility
                typeData += "," + "[]";                  //whatBuilds  谁建的？一般是农名建造，但如果是升级单位，则是前一个单位；兵则由建筑生产.
                typeData += "," + "[]";                  //requiredUnits 需要建筑，比如轨道指挥部需要兵营
                typeData += "," + "[]";                  //requiredUpgrades 
                typeData += ")";
                sb.AppendLine(string.Format("self.m_unitTypeData[{0}] = {1}", cfg.Id, typeData));
            }

            SaveFile(Application.dataPath + "/tech_tree.py", sb.ToString());
            //处理升级ID，如果某个建筑可以升级到。。。单独做成升级，比如UPGRADE_ID


        }