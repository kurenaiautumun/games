package com.example.quizprototype.Fragments

import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.quizprototype.DataClasses.Science
import com.example.quizprototype.Adapter.HorizontalAdapter
import com.example.quizprototype.R
import com.google.android.gms.ads.AdRequest
import com.google.android.gms.ads.AdView
import com.google.android.gms.ads.MobileAds
import com.google.firebase.FirebaseApp
import com.google.firebase.storage.FirebaseStorage

class HomeFragment : Fragment() {

    lateinit var mAdView : AdView
    val storage = FirebaseStorage.getInstance()
    val storageReference = storage.reference
    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {

        val view = inflater.inflate(R.layout.fragment_home, container, false)
        // Inflate the layout for this fragment

        MobileAds.initialize(requireContext()) {}
        FirebaseApp.initializeApp(requireContext())
        mAdView = view.findViewById(R.id.adView)
        val adRequest = AdRequest.Builder().build()
        mAdView.loadAd(adRequest)


        getdata("Science")
        getdata("Movies")
        getdata("GK")

        return view
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
            val recyclerView: RecyclerView? = view?.findViewById(R.id.scienceView)
            if (recyclerView != null) {
                recyclerView.layoutManager =
                    LinearLayoutManager(requireContext(), LinearLayoutManager.HORIZONTAL, false)
            }
            if (recyclerView != null) {
                recyclerView.adapter = HorizontalAdapter(requireContext(),list,sectionName)
            }
        }
        else if(sectionName=="Movies"){
            val recyclerView: RecyclerView? = view?.findViewById(R.id.moviesView)
            if (recyclerView != null) {
                recyclerView.layoutManager =
                    LinearLayoutManager(requireContext(), LinearLayoutManager.HORIZONTAL, false)
            }
            if (recyclerView != null) {
                recyclerView.adapter = HorizontalAdapter(requireContext(),list,sectionName)
            }
        }
        else if(sectionName=="GK"){
            val recyclerView: RecyclerView? = view?.findViewById(R.id.GKView)
            if (recyclerView != null) {
                recyclerView.layoutManager =
                    LinearLayoutManager(requireContext(), LinearLayoutManager.HORIZONTAL, false)
            }
            if (recyclerView != null) {
                recyclerView.adapter = HorizontalAdapter(requireContext(),list,sectionName)
            }
        }
    }


}