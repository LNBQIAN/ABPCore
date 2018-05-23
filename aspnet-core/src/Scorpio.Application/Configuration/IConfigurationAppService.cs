using System.Threading.Tasks;
using Scorpio.Configuration.Dto;

namespace Scorpio.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
