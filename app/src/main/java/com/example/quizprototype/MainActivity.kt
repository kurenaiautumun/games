package com.example.quizprototype

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.quizprototype.Adapter.HorizontalAdapter
import com.example.quizprototype.DataClasses.Science
import com.google.android.gms.ads.AdRequest
import com.google.android.gms.ads.AdView
import com.google.android.gms.ads.MobileAds
import com.google.firebase.FirebaseApp
import com.google.firebase.storage.FirebaseStorage

class MainActivity : AppCompatActivity() {

    lateinit var mAdView : AdView
    val storage = FirebaseStorage.getInstance()
    val storageReference = storage.reference
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        MobileAds.initialize(this) {}
        FirebaseApp.initializeApp(this)
        mAdView = findViewById(R.id.adView)
        val adRequest = AdRequest.Builder().build()
        mAdView.loadAd(adRequest)


        getdata("Science")
        getdata("Movies")
        getdata("GK")

    }
    fun getdata(sectionName : String){

        val list = mutableListOf<Science>()

        val sectionReference = storageReference.child(sectionName)


        sectionReference.listAll()
            .addOnSuccessListener { result ->
                for (item in result.items) {
                    // Get the metadata (name) for each image
                    item.metadata
                        .addOnSuccessListener { metadata ->
                            val imageName = metadata.name
                            // Get the download URL for each image
                            item.downloadUrl
                                .addOnSuccessListener { uri ->
                                    val downloadUrl = uri.toString()
                                    list.add(Science(downloadUrl,imageName.toString()))
                                    if(list.size==4){
                                        setadapter(list,sectionName)
                                    }
                                    else if(sectionName=="GK"){
                                        if(list.size==1){
                                            setadapter(list,sectionName)
                                        }
                                    }
                                }
                                .addOnFailureListener { exception ->
                                    // Handle errors
                                    println("Error getting download URL: $exception")
                                }
                        }
                        .addOnFailureListener { exception ->
                            // Handle errors
                            println("Error getting metadata: $exception")
                        }
                }
            }
            .addOnFailureListener { exception ->
                // Handle errors
                println("Error listing items: $exception")
            }

        Log.d("list",list.toString())
    }

    private fun setadapter(list: MutableList<Science>, sectionName: String) {

        if(sectionName=="Science") {
            val recyclerView: RecyclerView = findViewById(R.id.scienceView)
            recyclerView.layoutManager =
                LinearLayoutManager(this, LinearLayoutManager.HORIZONTAL, false)
            recyclerView.adapter = HorizontalAdapter(this,list,sectionName)
        }
        else if(sectionName=="Movies"){
            val recyclerView: RecyclerView = findViewById(R.id.moviesView)
            recyclerView.layoutManager =
                LinearLayoutManager(this, LinearLayoutManager.HORIZONTAL, false)
            recyclerView.adapter = HorizontalAdapter(this,list,sectionName)
        }
        else if(sectionName=="GK"){
            val recyclerView: RecyclerView = findViewById(R.id.GKView)
            recyclerView.layoutManager =
                LinearLayoutManager(this, LinearLayoutManager.HORIZONTAL, false)
            recyclerView.adapter = HorizontalAdapter(this,list,sectionName)
        }
    }
}