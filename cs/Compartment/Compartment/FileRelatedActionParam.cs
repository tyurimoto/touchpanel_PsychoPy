using Newtonsoft.Json;
using System;
using System.IO;


namespace Compartment
{
    public class FileRelatedActionParam
    {
        // Unused field - commented out to remove warning
        // private bool _Updated = false;
        public string ActionParams
        {
            get => _ActionParams ?? "";
            set => _ActionParams = value;
        }

        string _ActionParams;

        public string FilePath
        {
            get => _FilePath ?? "";
            set
            {
                _FilePath = value;
                // _Updated = false;
            }
        }

        string _FilePath;

        public FileRelatedActionParam()
        {

        }
        public void UpdateActionParam()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    try
                    {
                        string actionParamString = File.ReadAllText(FilePath);
                        ActionParams = actionParamString;
                        _Updated = true;
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }


            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool CompareActionParam(string actionParam)
        {
            return ActionParams == actionParam;
        }

        public bool CompareFileActionParam()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    string actionParamString = File.ReadAllText(FilePath);
                    return CompareActionParam(actionParamString);
                }
                catch (Exception)
                {
                    throw;
                }

            }

            return false;
        }

        public void SaveToJson()
        {
            try
            {
                File.WriteAllText(FilePath, ActionParams);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public UcOperationBlock.ActionParam[] GetActionParams()
        {
            try
            {
                var actionParams = JsonConvert.DeserializeObject<UcOperationBlock.ActionParam[]>(ActionParams);
                return actionParams;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
