using Google.Cloud.Firestore;

namespace ProyectoQ4_G3_Backend.Services;

public class FirebaseService
{
    private readonly FirestoreDb _firestoreDb;

    public FirebaseService()
    {
        var credentialsPath = Path.Combine(AppContext.BaseDirectory, "Config", "firebase-credentials.json");
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

        _firestoreDb = FirestoreDb.Create(projectId: "proyecto-beac2");
    }

    public CollectionReference GetCollection(string collectionName)
        {
            return _firestoreDb.Collection(collectionName);
        }
}