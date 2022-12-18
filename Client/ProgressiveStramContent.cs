using System.Net;

namespace CorporateDb.Client;

public class ProgressiveStreamContent : StreamContent
{
    private readonly Stream _fileStream;
    private readonly int _maxBuffer = 1024 * 4;

    public ProgressiveStreamContent(Stream stream, int maxBuffer, Action<long, double> onProgress) : base(stream)
    {
        _fileStream = stream;
        _maxBuffer = maxBuffer;
        OnProgress += onProgress;
    }

    
    public event Action<long, double> OnProgress;
    protected async override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
        var buffer = new byte[_maxBuffer];
        var totalLength = _fileStream.Length;
        long uploaded = 0;

        while (true)
        {
            using (_fileStream)
            {
                var length = await _fileStream.ReadAsync(buffer, 0, _maxBuffer);
                if (length <= 0)
                {
                    break;
                }

                uploaded += length;
                var perentage = Convert.ToDouble(uploaded * 100 / _fileStream.Length);

                await stream.WriteAsync(buffer);

                
                OnProgress?.Invoke(uploaded, percentage);

                
                await Task.Delay(250);
            }
        }
    }
    private long _uploaded = 0;
    private double _percentage = 0;
    public async Task SubmitFileAsync()
    {
        
        var content = new MultipartFormDataContent();
        var streamContent = new ProgressiveStreamContent(_fileStream, 40096, (u, p) =>
        {
          
            _uploaded = u;
            _percentage = p;


            //StateHasChanged();
        });

        content.Add(streamContent, "File");

        //var response = await Client.PostAsync("/weatherforecast", streamContent);
    }

}
