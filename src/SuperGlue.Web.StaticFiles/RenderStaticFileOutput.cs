﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperGlue.FileSystem;
using SuperGlue.Web.Output;

namespace SuperGlue.Web.StaticFiles
{
    public class RenderStaticFileOutput : IRenderOutput
    {
        public async Task<OutputRenderingResult> Render(IDictionary<string, object> environment)
        {
            var output = environment.GetOutput() as StaticFileOutput;

            if (output == null)
                return new OutputRenderingResult("", "");

            var fileSystem = environment.Resolve<IFileSystem>();

            var content = await fileSystem.ReadStringFromFile(output.FilePath).ConfigureAwait(false);

            return new OutputRenderingResult(content, environment.GetRequest().Headers.Accept.Split(',').Select(x => x.Trim()).FirstOrDefault());
        }
    }
}