package com.example.quizprototype.Fragments

import android.content.Context
import android.content.SharedPreferences
import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.appcompat.app.AlertDialog
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.quizprototype.DataClasses.Science
import com.example.quizprototype.Adapter.HorizontalAdapter
import com.example.quizprototype.DataClasses.User_Rank_Distribution
import com.example.quizprototype.R
import com.example.quizprototype.Response_Bodies.update_response
import com.example.quizprototype.Retrofit_Object_and_Interface.Retrofitobject
import com.google.android.gms.ads.AdRequest
import com.google.android.gms.ads.AdView
import com.google.android.gms.ads.MobileAds
import com.google.firebase.FirebaseApp
import com.google.firebase.appcheck.internal.util.Logger
import com.google.firebase.database.*
import com.google.firebase.storage.FirebaseStorage
import okhttp3.MultipartBody
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import java.lang.Long
import java.text.SimpleDateFormat
import java.util.*

class HomeFragment : Fragment() {

    lateinit var mAdView : AdView
    val storage = FirebaseStorage.getInstance()
    val storageReference = storage.reference
    private lateinit var sharedPreferences: SharedPreferences
    private lateinit var user_id : String
    private lateinit var quizname : String
    val quizzesRef: DatabaseReference = FirebaseDatabase.getInstance().getReference("Live_Quizes")
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
        sharedPreferences = requireContext().getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)
        user_id = sharedPreferences.getString("UID", "").toString()
        quizname = sharedPreferences.getString("quiz_name","").toString()

        updateReward(100,5)
//        reward()
        getdata("Science")
        getdata("Movies")
        getdata("GK")

        return view
    }

    private fun reward(reward: Int?) {


//        Toast.makeText(requireContext(),user_id,Toast.LENGTH_LONG).show()

        val requestBody = MultipartBody.Builder()
            .setType(MultipartBody.FORM)
            .addFormDataPart("user_id", user_id)
            .addFormDataPart("quiz_name",quizname)
            .build()

        Retrofitobject.apiInterface.getuserrank(requestBody).enqueue(object : Callback<User_Rank_Distribution?> {
            override fun onResponse(
                call: Call<User_Rank_Distribution?>,
                response: Response<User_Rank_Distribution?>
            ) {
                val total_size = response.body()?.total_size
                val rank = response.body()?.user_position

                if(total_size!=null && total_size!=0) {
                    val tenPercentOfTotalRank = (0.1 * total_size).toInt()
                    if((rank != null) && (rank != 0)){
                        if(rank<=tenPercentOfTotalRank){
                            val disttopperct = (0.7 * reward!!).toInt()
                            var divisiontoOne: Int
                            if(tenPercentOfTotalRank!=0) {
                                divisiontoOne = disttopperct / tenPercentOfTotalRank
                            }
                            else{
                                divisiontoOne  = disttopperct/total_size
                            }

//                            Toast.makeText(requireContext(),divisiontoOne.toString(),Toast.LENGTH_LONG).show()

                            updateWallet(divisiontoOne)
                        }
                        else{
                            val disttopperct = (0.3 * reward!!).toInt()
                            var divisiontoOne: Int
                            if(tenPercentOfTotalRank!=0) {
                                divisiontoOne = disttopperct / tenPercentOfTotalRank
                            }
                            else{
                                divisiontoOne  = disttopperct/total_size
                            }

//                            Toast.makeText(requireContext(),divisiontoOne.toString(),Toast.LENGTH_LONG).show()

                            updateWallet(divisiontoOne)

                        }
                    }

                }

            }

            override fun onFailure(call: Call<User_Rank_Distribution?>, t: Throwable) {
                Log.d("res","User Not Found")
            }
        })
    }

    private fun updateWallet(divisiontoOne: Int) {

        val number = sharedPreferences.getString("ph_number", "")

        val requestBody = MultipartBody.Builder()
            .setType(MultipartBody.FORM)
            .addFormDataPart("number", number.toString())
            .addFormDataPart("wallet","")
            .build()

        Retrofitobject.apiInterface.update(requestBody).enqueue(object : Callback<update_response?> {
            override fun onResponse(
                call: Call<update_response?>,
                response: Response<update_response?>
            ) {
                Log.d("res",response.body().toString())

                val Wallet = response.body()?.wallet.toString()
                updatingtoProfile(Wallet,divisiontoOne)
            }

            override fun onFailure(call: Call<update_response?>, t: Throwable) {

            }
        })
    }

    private fun updatingtoProfile(wallet: String,reward : Int) {
        val number = sharedPreferences.getString("ph_number", "")

        val rewardtowallet = (wallet.toInt() + reward).toString()

        val requestBody = MultipartBody.Builder()
            .setType(MultipartBody.FORM)
            .addFormDataPart("number", number.toString())
            .addFormDataPart("wallet",rewardtowallet)
            .build()

        Retrofitobject.apiInterface.update(requestBody).enqueue(object : Callback<update_response?> {
            override fun onResponse(
                call: Call<update_response?>,
                response: Response<update_response?>
            ) {
                Log.d("res",response.body().toString())
                showRewardDialog(reward)
            }

            override fun onFailure(call: Call<update_response?>, t: Throwable) {

            }
        })
    }

    private fun showRewardDialog(reward: Int) {
        val builder = AlertDialog.Builder(requireContext())
        builder.setTitle("Reward")
            .setMessage("Congratulations! You earned a reward of $reward")
            .setPositiveButton("OK") { dialog, _ ->
                // Dismiss the dialog when OK is clicked
                dialog.dismiss()
            }

        val dialog = builder.create()
        dialog.show()
    }

    private fun updateReward(total_size: Int?, rank: Int?) {


        val quizdata = quizzesRef.child(quizname)
        quizdata.addValueEventListener(object : ValueEventListener {
            override fun onDataChange(dataSnapshot: DataSnapshot) {

                val quizzesData = dataSnapshot.value

                Log.d("resp",quizzesData.toString())
                if (quizzesData != null) {

                        // Access values under each quiz
                        val startTime = dataSnapshot.child("startDate").getValue(String::class.java)
                        val endTime = dataSnapshot.child("endDate").getValue(String::class.java)
                        val reward = dataSnapshot.child("Reward").getValue(Int::class.java)

                        if(!isQuizActive(startTime.toString(),endTime.toString())){
                            reward(reward)
//                            Toast.makeText(requireContext(),reward.toString(),Toast.LENGTH_LONG).show()
                        }

                }

            }

            override fun onCancelled(databaseError: DatabaseError) {
                Log.w(Logger.TAG, "loadPost:onCancelled", databaseError.toException())
            }
        })
    }

    private fun isQuizActive(startDate: String, endDate: String): Boolean {
        try {
            Log.d("starttime", startDate)
            Log.d("endtime", endDate)
            val currentCalendar = Calendar.getInstance()
            val currentTime = currentCalendar.timeInMillis  // Use timeInMillis to get current time in milliseconds
            val dateFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.getDefault())

            val startCalendar = Calendar.getInstance()
            startCalendar.time = dateFormat.parse(startDate) ?: Date(0)
            val startDateMillis = startCalendar.timeInMillis

            val endCalendar = Calendar.getInstance()
            endCalendar.time = dateFormat.parse(endDate) ?: Date(0)
            val endDateMillis = endCalendar.timeInMillis


            // Ensure correct comparison, regardless of the order of startDate and endDate

            val startDateMillisAdjusted = startDateMillis + (5 * 60 * 60 * 1000) + (30 * 60 * 1000)
            val endDateMillisAdjusted = endDateMillis + (5 * 60 * 60 * 1000) + (30 * 60 * 1000)

            Log.d("time", "currentTime: $currentTime")
            Log.d("time", "startDateMillis: $startDateMillisAdjusted")
            Log.d("time", "endDateMillis: $endDateMillisAdjusted")

            val start = Long.min(startDateMillis, endDateMillis)
            val end = Long.max(startDateMillis, endDateMillis)

            return currentTime in start..end
        } catch (e: Exception) {
            e.printStackTrace()
            return false
        }
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