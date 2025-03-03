using DaemonMC.Forms.Components;
using DaemonMC.Network.Bedrock;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DaemonMC.Forms
{
    public static class FormManager
    {
        public static Dictionary<long, (Form form, Action<Player, string> callback)> PendingForms = new Dictionary<long, (Form form, Action<Player, string> callback)>();
        public static DefaultContractResolver contractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() };
    
        public static void Process(Player player, ModalFormResponse formResponse, (Form form, Action<Player, string> callback) entry)
        {
            var (form, callback) = entry;
            PendingForms.Remove(formResponse.ID);

            if (form is ModalForm modalForm && formResponse.Data != null)
            {
                if (formResponse.Data.Contains("true"))
                {
                    callback(player, modalForm.Button1);
                }
                else if (formResponse.Data.Contains("false"))
                {
                    callback(player, modalForm.Button2);
                }
            }

            if (form is SimpleForm simpleForm && formResponse.Data != null && int.TryParse(formResponse.Data, out int x))
            {
                callback(player, simpleForm.Buttons[x].Text);
            }

            if (form is CustomForm customForm && formResponse.Data != null)
            {
                object[] rawData = JsonConvert.DeserializeObject<object[]>(formResponse.Data) ?? Array.Empty<object>();
                string[] components = new string[customForm.Content.Count];

                for (int i = 0; i < customForm.Content.Count; i++)
                {
                    if (customForm.Content[i] is Label label)
                    {
                        components[i] = label.Text;
                    }

                    if (customForm.Content[i] is Dropdown dropdown)
                    {
                        components[i] = dropdown.Options[Convert.ToInt32(rawData[i])];
                    }

                    if (customForm.Content[i] is Input or Slider or Toggle)
                    {
                        components[i] = rawData[i].ToString() ?? "<NO DATA>";
                    }

                    if (customForm.Content[i] is StepSlider stepSlider)
                    {
                        components[i] = stepSlider.Steps[Convert.ToInt32(rawData[i])];
                    }
                }

                callback(player, JsonConvert.SerializeObject(components));
            }
        }
    }
}
