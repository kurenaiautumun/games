package com.example.quizprototype.Fragments

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
import com.example.quizprototype.DataClasses.stored_modifiedquestions
import com.example.quizprototype.Post_Quiz
import com.example.quizprototype.R
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken


class wrongFragment : Fragment() {

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.fragment_wrong, container, false)

        // Inflate the layout for this fragment
        val sharedPreferences = requireContext().getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)
        val gson = Gson()
        val json = sharedPreferences.getString("wrong", "")
        val storedListType = object : TypeToken<List<stored_modifiedquestions>>() {}.type
        var storedList: List<stored_modifiedquestions> = gson.fromJson(json, storedListType) ?: emptyList()

        val scienceindex = storedList.indexOfFirst { it.title == "Science"}
        val moviesindex = storedList.indexOfFirst { it.title == "Movies"}
        val gkindex = storedList.indexOfFirst { it.title == "GK"}
        var check1 = 0
        var check2 = 0
        var check3 = 0


        if(scienceindex==-1){
            check1 = 1
        }
        if(moviesindex==-1){
            check2 = 1
        }
        if(gkindex==-1){
            check3 = 1
        }

        Log.d("wronglist",storedList.toString())

        val SCIWrong = view.findViewById<ImageView>(R.id.scienceImageWrong)
        val MOVIEWrong = view.findViewById<ImageView>(R.id.moviesImageWrong)
        val GKsecWrong = view.findViewById<ImageView>(R.id.GKImageWrong)
        val sciTextWrong = view.findViewById<TextView>(R.id.scienceWrong)
        val movietextWrong = view.findViewById<TextView>(R.id.moviesWrong)
        val GKTextWrong = view.findViewById<TextView>(R.id.GKWrong)

        if(check1==1 || storedList[scienceindex].subsections.isEmpty()){
            SCIWrong.visibility = View.GONE
            sciTextWrong.visibility = View.GONE
        }
        if(check2==1|| storedList[moviesindex].subsections.isEmpty()){
            MOVIEWrong.visibility = View.GONE
            movietextWrong.visibility = View.GONE
        }
        if(check3==1|| storedList[gkindex].subsections.isEmpty()){
            GKsecWrong.visibility =View.GONE
            GKTextWrong.visibility = View.GONE
        }
//
        SCIWrong.setOnClickListener {
            val intent = Intent(requireContext(),Post_Quiz::class.java)
            intent.putExtra("section","wrong")
            intent.putExtra("category","Science")
            startActivity(intent)
        }
        MOVIEWrong.setOnClickListener {
            val intent = Intent(requireContext(),Post_Quiz::class.java)
            intent.putExtra("section","wrong")
            intent.putExtra("category","Movies")
            startActivity(intent)
        }
        GKsecWrong.setOnClickListener {
            val intent = Intent(requireContext(),Post_Quiz::class.java)
            intent.putExtra("section","wrong")
            intent.putExtra("category","GK")
            startActivity(intent)
        }

        return view
    }

}