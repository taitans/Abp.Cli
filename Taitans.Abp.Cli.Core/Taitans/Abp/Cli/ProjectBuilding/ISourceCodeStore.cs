using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Taitans.Abp.Cli.ProjectBuilding
{
    public interface ISourceCodeStore
    {
        Task<TemplateFile> GetAsync(
            string name,
            string type,
            [CanBeNull] string version = null
        );
    }
}