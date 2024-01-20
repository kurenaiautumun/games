package com.example.quizprototype.Fragments

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import com.example.quizprototype.DataClasses.Gk
import com.example.quizprototype.DataClasses.modifiedquestions
import com.example.quizprototype.DataClasses.stored_modifiedquestions
import com.example.quizprototype.Post_Quiz
import com.example.quizprototype.R
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken


class missesFragment : Fragment() {
    private lateinit var sharedPreferences: SharedPreferences
    private lateinit var SCI:ImageView
    private lateinit var MOVIE:ImageView
    private lateinit var GKsec :ImageView
    private lateinit var sciText:TextView
    private lateinit var movietext:TextView
    private lateinit var GKText :TextView
    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.fragment_misses, container, false)

        // Inflate the layout for this fragment
        val sharedPreferences = requireContext().getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)
        val gson = Gson()
        val json = sharedPreferences.getString("miss", "")
        val storedListType = object : TypeToken<List<stored_modifiedquestions>>() {}.type
        var storedList: List<stored_modifiedquestions> = gson.fromJson(json, storedListType) ?: emptyList()

        val scienceindex = storedList.indexOfFirst { it.title == "Science"}
        val moviesindex = storedList.indexOfFirst { it.title == "Movies"}
        val gkindex = storedList.indexOfFirst { it.title == "GK"}
        var check1 = 0
        var check2 = 0
        var check3 = 0

        if(scienceindex==-1){
            Toast.makeText(requireContext(),"1",Toast.LENGTH_LONG).show()
            check1 = 1
        }
        if(moviesindex==-1){
            check2 = 1
        }
        if(gkindex==-1){
            check3 = 1
        }

        Log.d("list",storedList.toString())
         SCI = view.findViewById<ImageView>(R.id.scienceImageMisses)
         MOVIE = view.findViewById<ImageView>(R.id.moviesImageMisses)
         GKsec = view.findViewById<ImageView>(R.id.GKImageMisses)
         sciText = view.findViewById<TextView>(R.id.scienceMisses)
         movietext = view.findViewById<TextView>(R.id.moviesMisses)
         GKText = view.findViewById<TextView>(R.id.GKMisses)

        if(check1==1 || storedList[scienceindex].subsections.isEmpty()){
            SCI.visibility = View.GONE
            sciText.visibility = View.GONE
        }
        if(check2==1|| storedList[moviesindex].subsections.isEmpty()){
            MOVIE.visibility = View.GONE
            movietext.visibility = View.GONE
        }
        if(check3==1|| storedList[gkindex].subsections.isEmpty()){
            GKsec.visibility =View.GONE
            GKText.visibility = View.GONE
        }


//
        SCI.setOnClickListener {
            val intent = Intent(requireContext(),Post_Quiz::class.java)
            intent.putExtra("section","miss")
            intent.putExtra("category","Science")
            startActivityForResult(intent, 1)
        }
        MOVIE.setOnClickListener {
            val intent = Intent(requireContext(),Post_Quiz::class.java)
            intent.putExtra("section","miss")
            intent.putExtra("category","Movies")
            startActivityForResult(intent, 1)
        }
        GKsec.setOnClickListener {
            val intent = Intent(requireContext(),Post_Quiz::class.java)
            intent.putExtra("section","miss")
            intent.putExtra("category","GK")
            startActivityForResult(intent, 1)
        }

        return view

    }

    fun retrieveStoredList(saveString: String): MutableList<stored_modifiedquestions> {
        val sharedPreferences = context?.getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)
        val storedJsonString = sharedPreferences?.getString(saveString, null)

        return if (storedJsonString != null) {
            Gson().fromJson(storedJsonString, object : TypeToken<MutableList<stored_modifiedquestions>>() {}.type)
        } else {
            mutableListOf()
        }
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (requestCode == 1 && resultCode == Activity.RESULT_OK) {
            // Handle the refresh logic here
            refreshFragment()
        }
    }

    private fun refreshFragment() {
        // Write the logic to refresh the fragment, e.g., re-fetch data
        Log.d("Refresh", "Fragment refreshed")
        val sharedPreferences = requireContext().getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)
        val gson = Gson()
        val json = sharedPreferences.getString("miss", "")
        val storedListType = object : TypeToken<List<stored_modifiedquestions>>() {}.type
        var storedList: List<stored_modifiedquestions> = gson.fromJson(json, storedListType) ?: emptyList()

        val scienceindex = storedList.indexOfFirst { it.title == "Science"}
        val moviesindex = storedList.indexOfFirst { it.title == "Movies"}
        val gkindex = storedList.indexOfFirst { it.title == "GK"}
        var check1 = 0
        var check2 = 0
        var check3 = 0

        if(scienceindex==-1){
            Toast.makeText(requireContext(),"1",Toast.LENGTH_LONG).show()
            check1 = 1
        }
        if(moviesindex==-1){
            check2 = 1
        }
        if(gkindex==-1){
            check3 = 1
        }

        Log.d("list",storedList.toString())

        if(check1==1 || storedList[scienceindex].subsections.isEmpty()){
            SCI.visibility = View.GONE
            sciText.visibility = View.GONE
        }
        if(check2==1|| storedList[moviesindex].subsections.isEmpty()){
            MOVIE.visibility = View.GONE
            movietext.visibility = View.GONE
        }
        if(check3==1|| storedList[gkindex].subsections.isEmpty()){
            GKsec.visibility =View.GONE
            GKText.visibility = View.GONE
        }

    }

    override fun onResume() {
        super.onResume()
        // Call the refresh method when the fragment is resumed
        refreshFragment()
    }

}