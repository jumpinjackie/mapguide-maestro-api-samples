using OSGeo.MapGuide.MaestroAPI;

namespace MvcCoreSample;

public delegate IServerConnection MgConnectionLoginFactory(string username, string password);
public delegate IServerConnection MgConnectionSessionFactory(string sessionId);