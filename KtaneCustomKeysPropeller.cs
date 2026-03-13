using RT.PropellerApi;
using RT.Servers;

namespace KtaneCustomKeys
{
    public sealed class KtaneCustomKeysPropeller : PropellerModuleBase<KtaneCustomKeysPropellerSettings>
    {
        public override string Name => "KTaNE Custom Keys Handler";

        public override HttpResponse Handle(HttpRequest req)
        {
            switch (req.Method)
            {
                case HttpMethod.Post:
                {
                    if (req.Post["data"].Value == null)
                        return HttpResponse.PlainText("Missing data.", HttpStatusCode._400_BadRequest);
                    return HttpResponse.PlainText(KtaneCustomKeysHolster.Push(req.Post["data"].Value));
                }
                case HttpMethod.Get:
                {
                    var headers = new HttpResponseHeaders { AccessControlAllowOrigin = "*" };

                    if (req.Url["code"] == null)
                        return HttpResponse.PlainText("Missing code.", HttpStatusCode._400_BadRequest, headers);

                    var result = KtaneCustomKeysHolster.Pull(req.Url["code"]);
                    if (result == null)
                        return HttpResponse.PlainText("Invalid code.", HttpStatusCode._404_NotFound, headers);
                    return HttpResponse.PlainText(result, HttpStatusCode._200_OK, headers);
                }
                case HttpMethod.Delete:
                {
                    if (req.Url["code"] == null || req.Url["token"] == null)
                        return HttpResponse.PlainText("Missing code or token.", HttpStatusCode._400_BadRequest);

                    var result = KtaneCustomKeysHolster.Remove(req.Url["code"], req.Url["token"]);
                    if (result == false)
                        return HttpResponse.PlainText("Invalid code.", HttpStatusCode._404_NotFound);
                    if (result == null)
                        return HttpResponse.PlainText("Invalid token.", HttpStatusCode._401_Unauthorized);
                    return HttpResponse.Empty();
                }
                default:
                    return HttpResponse.PlainText("Invalid request method.", HttpStatusCode._405_MethodNotAllowed);
            }
        }
    }

    public sealed class KtaneCustomKeysPropellerSettings { }
}
